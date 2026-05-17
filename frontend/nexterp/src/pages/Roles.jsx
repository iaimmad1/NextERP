import React, { useState, useEffect, useCallback } from 'react';
import { Container, Table, Button, Badge, Spinner, Alert } from 'react-bootstrap';
import { getRoles, deleteRole } from '../services/authService';
import { getStoredUser } from '../services/api';
import { useToast } from '../context/ToastContext';
import ConfirmModal from '../components/ConfirmModal';
import CreateRoleModal from '../components/roles/CreateRoleModal';
import EditRoleModal from '../components/roles/EditRoleModal';

const Roles = () => {
  const { showToast } = useToast();
  const currentUser = getStoredUser();
  const perms = new Set(currentUser?.permissions || []);
  const isAdmin = currentUser?.roles?.includes('Admin');
  const can = (p) => isAdmin || perms.has(p);

  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [showCreate, setShowCreate] = useState(false);
  const [editRole, setEditRole] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const fetchRoles = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getRoles();
      if (!res.success) throw new Error(res.message);
      setRoles(res.data || []);
    } catch (err) {
      if (err.status === 403) setError('You do not have permission to view roles.');
      else setError(err.message || 'Failed to load roles.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchRoles(); }, [fetchRoles]);

  const handleDelete = async () => {
    setDeleting(true);
    try {
      const res = await deleteRole(confirmDelete.roleId);
      if (!res.success) throw new Error(res.message);
      showToast('Role deleted successfully');
      setConfirmDelete(null);
      fetchRoles();
    } catch (err) {
      showToast(err.message || 'Delete failed', 'danger');
    } finally {
      setDeleting(false);
    }
  };

  return (
    <Container className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4 className="mb-0">Roles & Permissions</h4>
        {can('roles.create') && (
          <Button variant="primary" onClick={() => setShowCreate(true)}>+ New Role</Button>
        )}
      </div>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center py-5"><Spinner animation="border" /></div>
      ) : (
        <Table striped hover responsive className="shadow-sm align-middle">
          <thead className="table-dark">
            <tr>
              <th>#</th>
              <th>Role Name</th>
              <th>Description</th>
              <th>Users</th>
              <th>Type</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {roles.length === 0 ? (
              <tr><td colSpan={7} className="text-center text-muted py-4">No roles found.</td></tr>
            ) : roles.map((role, idx) => (
              <tr key={role.roleId}>
                <td>{idx + 1}</td>
                <td><strong>{role.roleName}</strong></td>
                <td className="text-muted">{role.description || '—'}</td>
                <td>
                  <Badge bg="light" text="dark" className="border">{role.userCount ?? 0}</Badge>
                </td>
                <td>
                  {role.isSystemRole
                    ? <Badge bg="secondary">System</Badge>
                    : <Badge bg="info">Custom</Badge>}
                </td>
                <td>
                  {(role.status || '').toLowerCase() === 'active'
                    ? <Badge bg="success">Active</Badge>
                    : <Badge bg="warning" text="dark">{role.status || 'Active'}</Badge>}
                </td>
                <td>
                  <div className="d-flex gap-1">
                    {can('roles.edit') && (
                      <Button size="sm" variant="outline-secondary" onClick={() => setEditRole(role)}>
                        Edit
                      </Button>
                    )}
                    {can('roles.delete') && !role.isSystemRole && (
                      <Button size="sm" variant="outline-danger" onClick={() => setConfirmDelete(role)}>
                        Delete
                      </Button>
                    )}
                    {role.isSystemRole && (
                      <span className="text-muted small align-self-center">Protected</span>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}

      <CreateRoleModal
        show={showCreate}
        onHide={() => setShowCreate(false)}
        onSuccess={fetchRoles}
      />

      <EditRoleModal
        show={!!editRole}
        onHide={() => setEditRole(null)}
        onSuccess={fetchRoles}
        role={editRole}
      />

      <ConfirmModal
        show={!!confirmDelete}
        onHide={() => setConfirmDelete(null)}
        onConfirm={handleDelete}
        title="Delete Role"
        message={`Are you sure you want to delete the role "${confirmDelete?.roleName}"? Users assigned this role will lose its permissions.`}
        loading={deleting}
      />
    </Container>
  );
};

export default Roles;
