import React, { useState } from 'react';
import { Modal, Form, Button, Alert } from 'react-bootstrap';
import { createRole } from '../../services/authService';
import { useToast } from '../../context/ToastContext';

const CreateRoleModal = ({ show, onHide, onSuccess }) => {
  const { showToast } = useToast();
  const [form, setForm] = useState({ name: '', description: '' });
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handle = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await createRole(form);
      if (!res.success) throw new Error(res.message);
      showToast('Role created successfully');
      setForm({ name: '', description: '' });
      onSuccess();
      onHide();
    } catch (err) {
      setError(err.message || 'Failed to create role');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal show={show} onHide={onHide} centered>
      <Form onSubmit={handleSubmit}>
        <Modal.Header closeButton>
          <Modal.Title>Create New Role</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form.Group className="mb-3" controlId="cr-name">
            <Form.Label>Role Name <span className="text-danger">*</span></Form.Label>
            <Form.Control name="name" value={form.name} onChange={handle} required placeholder="e.g. Manager" />
          </Form.Group>
          <Form.Group controlId="cr-description">
            <Form.Label>Description</Form.Label>
            <Form.Control
              as="textarea" rows={3}
              name="description" value={form.description} onChange={handle}
              placeholder="What can this role do?"
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onHide} disabled={loading}>Cancel</Button>
          <Button type="submit" variant="primary" disabled={loading}>
            {loading ? 'Creating...' : 'Create Role'}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
};

export default CreateRoleModal;
