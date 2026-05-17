import React, { useState, useEffect } from 'react';
import { Modal, Form, Button, Alert, Spinner, Badge, Tab, Nav, Row, Col } from 'react-bootstrap';
import {
  updateRole, getRoleById, getPermissions,
  assignPermission, revokePermission
} from '../../services/authService';
import { useToast } from '../../context/ToastContext';

const EditRoleModal = ({ show, onHide, onSuccess, role }) => {
  const { showToast } = useToast();
  const [form, setForm] = useState({ name: '', description: '' });
  const [allPermissions, setAllPermissions] = useState([]); // grouped: [{category, permissions:[]}]
  const [currentPermIds, setCurrentPermIds] = useState(new Set());
  const [selected, setSelected] = useState(new Set());
  const [loadingPerms, setLoadingPerms] = useState(false);
  const [saving, setSaving] = useState(false);
  const [savingPerms, setSavingPerms] = useState(false);
  const [error, setError] = useState(null);
  const [permError, setPermError] = useState(null);

  useEffect(() => {
    if (!show || !role) return;
    setForm({ name: role.roleName || '', description: role.description || '' });
    setError(null);
    setPermError(null);

    // Load role details (with permissions) + all permissions
    setLoadingPerms(true);
    Promise.all([getRoleById(role.roleId), getPermissions()])
      .then(([roleRes, permRes]) => {
        const rolePerms = (roleRes.data?.permissions || []).map(p => p.permissionId);
        const rolePermSet = new Set(rolePerms);
        setCurrentPermIds(new Set(rolePermSet));
        setSelected(new Set(rolePermSet));
        setAllPermissions(permRes.data || []);
      })
      .catch(err => setPermError(err.message || 'Failed to load permissions'))
      .finally(() => setLoadingPerms(false));
  }, [show, role]);

  const handle = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const togglePerm = (permId) =>
    setSelected(prev => {
      const next = new Set(prev);
      next.has(permId) ? next.delete(permId) : next.add(permId);
      return next;
    });

  const toggleCategory = (perms) => {
    const allChecked = perms.every(p => selected.has(p.permissionId));
    setSelected(prev => {
      const next = new Set(prev);
      perms.forEach(p => allChecked ? next.delete(p.permissionId) : next.add(p.permissionId));
      return next;
    });
  };

  const handleSaveDetails = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      const res = await updateRole(role.roleId, form);
      if (!res.success) throw new Error(res.message);
      showToast('Role updated successfully');
      onSuccess();
    } catch (err) {
      setError(err.message || 'Failed to update role');
    } finally {
      setSaving(false);
    }
  };

  const handleSavePermissions = async () => {
    setPermError(null);
    setSavingPerms(true);
    try {
      const toAssign = [...selected].filter(id => !currentPermIds.has(id));
      const toRevoke = [...currentPermIds].filter(id => !selected.has(id));
      await Promise.all([
        ...toAssign.map(id => assignPermission(role.roleId, id)),
        ...toRevoke.map(id => revokePermission(role.roleId, id)),
      ]);
      setCurrentPermIds(new Set(selected));
      showToast('Permissions updated successfully');
      onSuccess();
    } catch (err) {
      setPermError(err.message || 'Failed to update permissions');
    } finally {
      setSavingPerms(false);
    }
  };

  if (!role) return null;

  return (
    <Modal show={show} onHide={onHide} size="lg" centered>
      <Modal.Header closeButton>
        <Modal.Title>
          Edit Role — {role.roleName}
          {role.isSystemRole && <Badge bg="secondary" className="ms-2">System</Badge>}
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Tab.Container defaultActiveKey="details">
          <Nav variant="tabs" className="mb-3">
            <Nav.Item><Nav.Link eventKey="details">Details</Nav.Link></Nav.Item>
            <Nav.Item><Nav.Link eventKey="permissions">Permissions</Nav.Link></Nav.Item>
          </Nav>

          <Tab.Content>
            {/* ── Details tab ── */}
            <Tab.Pane eventKey="details">
              {error && <Alert variant="danger">{error}</Alert>}
              <Form onSubmit={handleSaveDetails}>
                <Form.Group className="mb-3" controlId="er-name">
                  <Form.Label>Role Name <span className="text-danger">*</span></Form.Label>
                  <Form.Control
                    name="name" value={form.name} onChange={handle}
                    required disabled={role.isSystemRole}
                  />
                  {role.isSystemRole && (
                    <Form.Text className="text-muted">System role names cannot be changed.</Form.Text>
                  )}
                </Form.Group>
                <Form.Group className="mb-3" controlId="er-desc">
                  <Form.Label>Description</Form.Label>
                  <Form.Control
                    as="textarea" rows={3}
                    name="description" value={form.description} onChange={handle}
                  />
                </Form.Group>
                <Button type="submit" variant="primary" disabled={saving}>
                  {saving ? 'Saving...' : 'Save Details'}
                </Button>
              </Form>
            </Tab.Pane>

            {/* ── Permissions tab ── */}
            <Tab.Pane eventKey="permissions">
              {permError && <Alert variant="danger">{permError}</Alert>}
              {loadingPerms ? (
                <div className="text-center py-4"><Spinner animation="border" size="sm" /></div>
              ) : (
                <>
                  {allPermissions.map(group => (
                    <div key={group.category} className="mb-4">
                      <div className="d-flex align-items-center gap-2 mb-2">
                        <Form.Check
                          type="checkbox"
                          id={`cat-${group.category}`}
                          checked={group.permissions.every(p => selected.has(p.permissionId))}
                          onChange={() => toggleCategory(group.permissions)}
                        />
                        <h6 className="mb-0 text-uppercase text-muted" style={{ fontSize: '0.75rem', letterSpacing: '0.05em' }}>
                          {group.category}
                        </h6>
                      </div>
                      <Row>
                        {group.permissions.map(perm => (
                          <Col md={6} key={perm.permissionId} className="mb-1">
                            <Form.Check
                              type="checkbox"
                              id={`perm-${perm.permissionId}`}
                              label={
                                <span>
                                  {perm.displayName}
                                  <small className="text-muted ms-1">({perm.permissionName})</small>
                                </span>
                              }
                              checked={selected.has(perm.permissionId)}
                              onChange={() => togglePerm(perm.permissionId)}
                            />
                          </Col>
                        ))}
                      </Row>
                    </div>
                  ))}
                  <Button variant="primary" onClick={handleSavePermissions} disabled={savingPerms}>
                    {savingPerms ? 'Saving...' : 'Save Permissions'}
                  </Button>
                </>
              )}
            </Tab.Pane>
          </Tab.Content>
        </Tab.Container>
      </Modal.Body>
    </Modal>
  );
};

export default EditRoleModal;
