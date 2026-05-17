import React, { useState } from 'react';
import { Modal, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { createUser } from '../../services/authService';
import { useToast } from '../../context/ToastContext';

const GENDERS = ['Male', 'Female', 'Other', 'PreferNotToSay'];

const initialForm = {
  username: '', email: '', password: '',
  firstName: '', middleName: '', lastName: '',
  phoneNumber: '', dateOfBirth: '', gender: '', address: '',
  roleNames: [],
};

const CreateUserModal = ({ show, onHide, onSuccess, availableRoles = [] }) => {
  const { showToast } = useToast();
  const [form, setForm] = useState(initialForm);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handle = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const toggleRole = (roleName) =>
    setForm(f => ({
      ...f,
      roleNames: f.roleNames.includes(roleName)
        ? f.roleNames.filter(r => r !== roleName)
        : [...f.roleNames, roleName],
    }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const payload = { ...form, dateOfBirth: form.dateOfBirth || '1990-01-01' };
      const res = await createUser(payload);
      if (!res.success) throw new Error(res.message);
      showToast('User created successfully');
      setForm(initialForm);
      onSuccess();
      onHide();
    } catch (err) {
      setError(err.message || 'Failed to create user');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal show={show} onHide={onHide} size="lg" centered>
      <Form onSubmit={handleSubmit}>
        <Modal.Header closeButton>
          <Modal.Title>Create New User</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {error && <Alert variant="danger">{error}</Alert>}

          <h6 className="text-muted mb-3">Account Details</h6>
          <Row className="mb-3">
            <Col md={6}>
              <Form.Group controlId="cu-username">
                <Form.Label>Username <span className="text-danger">*</span></Form.Label>
                <Form.Control name="username" value={form.username} onChange={handle} required />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group controlId="cu-email">
                <Form.Label>Email <span className="text-danger">*</span></Form.Label>
                <Form.Control type="email" name="email" value={form.email} onChange={handle} required />
              </Form.Group>
            </Col>
          </Row>
          <Row className="mb-3">
            <Col md={6}>
              <Form.Group controlId="cu-password">
                <Form.Label>Password <span className="text-danger">*</span></Form.Label>
                <Form.Control type="password" name="password" value={form.password} onChange={handle} required />
              </Form.Group>
            </Col>
          </Row>

          <h6 className="text-muted mb-3 mt-2">Personal Details</h6>
          <Row className="mb-3">
            <Col md={4}>
              <Form.Group controlId="cu-firstName">
                <Form.Label>First Name <span className="text-danger">*</span></Form.Label>
                <Form.Control name="firstName" value={form.firstName} onChange={handle} required />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="cu-middleName">
                <Form.Label>Middle Name</Form.Label>
                <Form.Control name="middleName" value={form.middleName} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="cu-lastName">
                <Form.Label>Last Name <span className="text-danger">*</span></Form.Label>
                <Form.Control name="lastName" value={form.lastName} onChange={handle} required />
              </Form.Group>
            </Col>
          </Row>
          <Row className="mb-3">
            <Col md={4}>
              <Form.Group controlId="cu-phone">
                <Form.Label>Phone Number</Form.Label>
                <Form.Control name="phoneNumber" value={form.phoneNumber} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="cu-dob">
                <Form.Label>Date of Birth</Form.Label>
                <Form.Control type="date" name="dateOfBirth" value={form.dateOfBirth} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="cu-gender">
                <Form.Label>Gender</Form.Label>
                <Form.Select name="gender" value={form.gender} onChange={handle}>
                  <option value="">Select...</option>
                  {GENDERS.map(g => <option key={g} value={g}>{g}</option>)}
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>
          <Form.Group className="mb-3" controlId="cu-address">
            <Form.Label>Address</Form.Label>
            <Form.Control name="address" value={form.address} onChange={handle} />
          </Form.Group>

          {availableRoles.length > 0 && (
            <>
              <h6 className="text-muted mb-2 mt-2">Assign Roles</h6>
              <div className="d-flex flex-wrap gap-3">
                {availableRoles.map(role => (
                  <Form.Check
                    key={role.roleId}
                    type="checkbox"
                    id={`cu-role-${role.roleId}`}
                    label={role.roleName}
                    checked={form.roleNames.includes(role.roleName)}
                    onChange={() => toggleRole(role.roleName)}
                  />
                ))}
              </div>
            </>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onHide} disabled={loading}>Cancel</Button>
          <Button type="submit" variant="primary" disabled={loading}>
            {loading ? 'Creating...' : 'Create User'}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
};

export default CreateUserModal;
