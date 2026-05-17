import React, { useState, useEffect } from 'react';
import { Modal, Button, Spinner, Alert, Badge, ListGroup, Form } from 'react-bootstrap';
import { getRoles, assignRoleToUser, removeRoleFromUser } from '../../services/authService';
import { useToast } from '../../context/ToastContext';

const UserRolesModal = ({ show, onHide, onSuccess, user }) => {
  const { showToast } = useToast();
  const [allRoles, setAllRoles] = useState([]);
  const [selected, setSelected] = useState(new Set());
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!show || !user) return;
    setLoading(true);
    getRoles()
      .then(res => {
        const roles = res.data || [];
        setAllRoles(roles);
        // Pre-check roles the user already has (by name match)
        const userRoleNames = new Set(user.roles || []);
        const preSelected = new Set(
          roles.filter(r => userRoleNames.has(r.roleName)).map(r => r.roleId)
        );
        setSelected(preSelected);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [show, user]);

  const toggle = (roleId) =>
    setSelected(prev => {
      const next = new Set(prev);
      next.has(roleId) ? next.delete(roleId) : next.add(roleId);
      return next;
    });

  const handleSave = async () => {
    setSaving(true);
    setError(null);
    try {
      const userRoleNames = new Set(user.roles || []);
      const previousIds = new Set(
        allRoles.filter(r => userRoleNames.has(r.roleName)).map(r => r.roleId)
      );

      const toAssign = [...selected].filter(id => !previousIds.has(id));
      const toRemove = [...previousIds].filter(id => !selected.has(id));

      await Promise.all([
        ...toAssign.map(id => assignRoleToUser(user.userId, id)),
        ...toRemove.map(id => removeRoleFromUser(user.userId, id)),
      ]);

      showToast('Roles updated successfully');
      onSuccess();
      onHide();
    } catch (err) {
      setError(err.message || 'Failed to update roles');
    } finally {
      setSaving(false);
    }
  };

  if (!user) return null;

  return (
    <Modal show={show} onHide={onHide} centered>
      <Modal.Header closeButton>
        <Modal.Title>Manage Roles — {user.username}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {error && <Alert variant="danger">{error}</Alert>}
        {loading ? (
          <div className="text-center py-3"><Spinner animation="border" size="sm" /></div>
        ) : (
          <ListGroup>
            {allRoles.map(role => (
              <ListGroup.Item key={role.roleId} className="d-flex align-items-center gap-3">
                <Form.Check
                  type="checkbox"
                  id={`role-check-${role.roleId}`}
                  checked={selected.has(role.roleId)}
                  onChange={() => toggle(role.roleId)}
                />
                <div>
                  <div className="fw-semibold">{role.roleName}</div>
                  {role.description && <small className="text-muted">{role.description}</small>}
                </div>
                {role.isSystemRole && <Badge bg="secondary" className="ms-auto">System</Badge>}
              </ListGroup.Item>
            ))}
          </ListGroup>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide} disabled={saving}>Cancel</Button>
        <Button variant="primary" onClick={handleSave} disabled={saving || loading}>
          {saving ? 'Saving...' : 'Save Roles'}
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default UserRolesModal;
