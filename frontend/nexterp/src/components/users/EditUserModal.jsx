import React, { useState, useEffect } from 'react';
import { Modal, Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { updateUser } from '../../services/authService';
import { useToast } from '../../context/ToastContext';

const GENDERS = ['Male', 'Female', 'Other', 'PreferNotToSay'];

const EditUserModal = ({ show, onHide, onSuccess, user }) => {
  const { showToast } = useToast();
  const [form, setForm] = useState({
    firstName: '', middleName: '', lastName: '',
    phoneNumber: '', dateOfBirth: '', gender: '', address: '',
  });
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (user) {
      setForm({
        firstName: user.firstName || '',
        middleName: user.middleName || '',
        lastName: user.lastName || '',
        phoneNumber: user.phoneNumber || '',
        dateOfBirth: user.dateOfBirth ? user.dateOfBirth.split('T')[0] : '',
        gender: user.gender || '',
        address: user.address || '',
      });
      setError(null);
    }
  }, [user]);

  const handle = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await updateUser(user.userId, form);
      if (!res.success) throw new Error(res.message);
      showToast('User updated successfully');
      onSuccess();
      onHide();
    } catch (err) {
      setError(err.message || 'Failed to update user');
    } finally {
      setLoading(false);
    }
  };

  if (!user) return null;

  return (
    <Modal show={show} onHide={onHide} size="lg" centered>
      <Form onSubmit={handleSubmit}>
        <Modal.Header closeButton>
          <Modal.Title>Edit User — {user.username}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          <Row className="mb-3">
            <Col md={4}>
              <Form.Group controlId="eu-firstName">
                <Form.Label>First Name</Form.Label>
                <Form.Control name="firstName" value={form.firstName} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="eu-middleName">
                <Form.Label>Middle Name</Form.Label>
                <Form.Control name="middleName" value={form.middleName} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="eu-lastName">
                <Form.Label>Last Name</Form.Label>
                <Form.Control name="lastName" value={form.lastName} onChange={handle} />
              </Form.Group>
            </Col>
          </Row>
          <Row className="mb-3">
            <Col md={4}>
              <Form.Group controlId="eu-phone">
                <Form.Label>Phone Number</Form.Label>
                <Form.Control name="phoneNumber" value={form.phoneNumber} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="eu-dob">
                <Form.Label>Date of Birth</Form.Label>
                <Form.Control type="date" name="dateOfBirth" value={form.dateOfBirth} onChange={handle} />
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group controlId="eu-gender">
                <Form.Label>Gender</Form.Label>
                <Form.Select name="gender" value={form.gender} onChange={handle}>
                  <option value="">Select...</option>
                  {GENDERS.map(g => <option key={g} value={g}>{g}</option>)}
                </Form.Select>
              </Form.Group>
            </Col>
          </Row>
          <Form.Group controlId="eu-address">
            <Form.Label>Address</Form.Label>
            <Form.Control name="address" value={form.address} onChange={handle} />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onHide} disabled={loading}>Cancel</Button>
          <Button type="submit" variant="primary" disabled={loading}>
            {loading ? 'Saving...' : 'Save Changes'}
          </Button>
        </Modal.Footer>
      </Form>
    </Modal>
  );
};

export default EditUserModal;
