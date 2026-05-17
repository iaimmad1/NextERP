import React, { useState, useEffect, useCallback } from 'react';
import {
  Container, Table, Button, Badge, Spinner, Alert,
  InputGroup, Form, Pagination
} from 'react-bootstrap';
import { getUsers, deleteUser, setUserStatus, getRoles } from '../services/authService';
import { getStoredUser } from '../services/api';
import { useToast } from '../context/ToastContext';
import ConfirmModal from '../components/ConfirmModal';
import CreateUserModal from '../components/users/CreateUserModal';
import EditUserModal from '../components/users/EditUserModal';
import UserRolesModal from '../components/users/UserRolesModal';

const PAGE_SIZE = 10;

const statusBadge = (status) => {
  const s = (status || '').toLowerCase();
  if (s === 'active') return <Badge bg="success">Active</Badge>;
  if (s === 'inactive') return <Badge bg="secondary">Inactive</Badge>;
  return <Badge bg="warning" text="dark">{status}</Badge>;
};

const Users = () => {
  const { showToast } = useToast();
  const currentUser = getStoredUser();
  const perms = new Set(currentUser?.permissions || []);
  const isAdmin = currentUser?.roles?.includes('Admin');
  const can = (p) => isAdmin || perms.has(p);

  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState('');
  const [availableRoles, setAvailableRoles] = useState([]);

  // Modal state
  const [showCreate, setShowCreate] = useState(false);
  const [editUser, setEditUser] = useState(null);
  const [rolesUser, setRolesUser] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(null);
  const [confirmStatus, setConfirmStatus] = useState(null);
  const [actionLoading, setActionLoading] = useState(false);

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getUsers(page, PAGE_SIZE);
      if (!res.success) throw new Error(res.message);
      const payload = res.data;
      setUsers(payload?.items || (Array.isArray(payload) ? payload : []));
      setTotalCount(payload?.totalCount ?? (Array.isArray(payload) ? payload.length : 0));
    } catch (err) {
      if (err.status === 403) setError('You do not have permission to view users.');
      else setError(err.message || 'Failed to load users.');
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => { fetchUsers(); }, [fetchUsers]);

  useEffect(() => {
    if (can('roles.view')) {
      getRoles().then(res => { if (res.success) setAvailableRoles(res.data || []); }).catch(() => {});
    }
  }, []); // eslint-disable-line

  // Delete
  const handleDelete = async () => {
    setActionLoading(true);
    try {
      const res = await deleteUser(confirmDelete.userId);
      if (!res.success) throw new Error(res.message);
      showToast('User deleted successfully');
      setConfirmDelete(null);
      fetchUsers();
    } catch (err) {
      showToast(err.message || 'Delete failed', 'danger');
    } finally {
      setActionLoading(false);
    }
  };

  // Toggle status
  const handleToggleStatus = async () => {
    const { user } = confirmStatus;
    const newStatus = user.status?.toLowerCase() === 'active' ? 'Inactive' : 'Active';
    setActionLoading(true);
    try {
      const res = await setUserStatus(user.userId, newStatus);
      if (!res.success) throw new Error(res.message);
      showToast(`User ${newStatus === 'Active' ? 'activated' : 'deactivated'}`);
      setConfirmStatus(null);
      fetchUsers();
    } catch (err) {
      showToast(err.message || 'Status update failed', 'danger');
    } finally {
      setActionLoading(false);
    }
  };

  // Filtered (client-side search)
  const filtered = users.filter(u =>
    !search ||
    u.username?.toLowerCase().includes(search.toLowerCase()) ||
    u.email?.toLowerCase().includes(search.toLowerCase()) ||
    u.fullName?.toLowerCase().includes(search.toLowerCase())
  );

  const totalPages = Math.ceil(totalCount / PAGE_SIZE);

  return (
    <Container className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4 className="mb-0">Users Management</h4>
        {can('users.create') && (
          <Button variant="primary" onClick={() => setShowCreate(true)}>+ New User</Button>
        )}
      </div>

      <InputGroup className="mb-3" style={{ maxWidth: 340 }}>
        <Form.Control
          placeholder="Search by name, email, username…"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        {search && (
          <Button variant="outline-secondary" onClick={() => setSearch('')}>✕</Button>
        )}
      </InputGroup>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center py-5"><Spinner animation="border" /></div>
      ) : (
        <>
          <Table striped hover responsive className="shadow-sm align-middle">
            <thead className="table-dark">
              <tr>
                <th>#</th>
                <th>Username</th>
                <th>Full Name</th>
                <th>Email</th>
                <th>Roles</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr><td colSpan={7} className="text-center text-muted py-4">No users found.</td></tr>
              ) : filtered.map((user, idx) => (
                <tr key={user.userId}>
                  <td>{(page - 1) * PAGE_SIZE + idx + 1}</td>
                  <td><strong>{user.username}</strong></td>
                  <td>{user.fullName}</td>
                  <td>{user.email}</td>
                  <td>
                    {user.roles?.length > 0
                      ? user.roles.map(r => <Badge bg="info" className="me-1" key={r}>{r}</Badge>)
                      : <span className="text-muted">—</span>}
                  </td>
                  <td>{statusBadge(user.status)}</td>
                  <td>
                    <div className="d-flex gap-1 flex-wrap">
                      {can('roles.assign') && (
                        <Button size="sm" variant="outline-primary" onClick={() => setRolesUser(user)}>
                          Roles
                        </Button>
                      )}
                      {can('users.edit') && (
                        <Button size="sm" variant="outline-secondary" onClick={() => setEditUser(user)}>
                          Edit
                        </Button>
                      )}
                      {can('users.manage') && (
                        <Button
                          size="sm"
                          variant={user.status?.toLowerCase() === 'active' ? 'outline-warning' : 'outline-success'}
                          onClick={() => setConfirmStatus({ user })}
                        >
                          {user.status?.toLowerCase() === 'active' ? 'Deactivate' : 'Activate'}
                        </Button>
                      )}
                      {can('users.delete') && (
                        <Button size="sm" variant="outline-danger" onClick={() => setConfirmDelete(user)}>
                          Delete
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>

          {totalPages > 1 && (
            <Pagination className="justify-content-center">
              <Pagination.Prev disabled={page === 1} onClick={() => setPage(p => p - 1)} />
              {[...Array(totalPages)].map((_, i) => (
                <Pagination.Item key={i + 1} active={page === i + 1} onClick={() => setPage(i + 1)}>
                  {i + 1}
                </Pagination.Item>
              ))}
              <Pagination.Next disabled={page === totalPages} onClick={() => setPage(p => p + 1)} />
            </Pagination>
          )}
        </>
      )}

      {/* Modals */}
      <CreateUserModal
        show={showCreate}
        onHide={() => setShowCreate(false)}
        onSuccess={fetchUsers}
        availableRoles={availableRoles}
      />

      <EditUserModal
        show={!!editUser}
        onHide={() => setEditUser(null)}
        onSuccess={fetchUsers}
        user={editUser}
      />

      <UserRolesModal
        show={!!rolesUser}
        onHide={() => setRolesUser(null)}
        onSuccess={fetchUsers}
        user={rolesUser}
      />

      <ConfirmModal
        show={!!confirmDelete}
        onHide={() => setConfirmDelete(null)}
        onConfirm={handleDelete}
        title="Delete User"
        message={`Are you sure you want to delete "${confirmDelete?.username}"? This action cannot be undone.`}
        loading={actionLoading}
      />

      <ConfirmModal
        show={!!confirmStatus}
        onHide={() => setConfirmStatus(null)}
        onConfirm={handleToggleStatus}
        title={confirmStatus?.user?.status?.toLowerCase() === 'active' ? 'Deactivate User' : 'Activate User'}
        message={`Are you sure you want to ${confirmStatus?.user?.status?.toLowerCase() === 'active' ? 'deactivate' : 'activate'} "${confirmStatus?.user?.username}"?`}
        confirmVariant={confirmStatus?.user?.status?.toLowerCase() === 'active' ? 'warning' : 'success'}
        confirmLabel={confirmStatus?.user?.status?.toLowerCase() === 'active' ? 'Deactivate' : 'Activate'}
        loading={actionLoading}
      />
    </Container>
  );
};

export default Users;
