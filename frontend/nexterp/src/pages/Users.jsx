import React, { useEffect, useState } from 'react';
import { Container, Table, Spinner, Alert, Badge } from 'react-bootstrap';
import { getUsers } from '../services/authService';

const Users = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await getUsers(1, 10);
        if (!response.success) throw new Error(response.message);
        // Backend returns PagedResponse<UserResponseDto> which has an 'items' property
        const payload = response.data;
        setUsers(payload?.items || (Array.isArray(payload) ? payload : []));
      } catch (err) {
        if (err.status === 403) {
          setError('Forbidden: You do not have permission to view users.');
        } else {
          setError(err.message || 'Failed to load users.');
        }
      } finally {
        setLoading(false);
      }
    };
    fetchUsers();
  }, []);

  if (loading) {
    return (
      <Container className="d-flex justify-content-center mt-5">
        <Spinner animation="border" variant="primary" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="mt-5">
        <Alert variant="danger">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container className="mt-5">
      <h2>Users Management</h2>
      <Table striped bordered hover className="mt-4 shadow-sm">
        <thead className="table-dark">
          <tr>
            <th>Username</th>
            <th>Email</th>
            <th>Full Name</th>
            <th>Roles</th>
          </tr>
        </thead>
        <tbody>
          {users && users.length > 0 ? (
            users.map(user => (
              <tr key={user.id || user.username}>
                <td>{user.username}</td>
                <td>{user.email}</td>
                <td>{user.fullName}</td>
                <td>
                  {user.roles && user.roles.length > 0 ? (
                    user.roles.map(role => (
                      <Badge bg="info" className="me-1" key={role}>{role}</Badge>
                    ))
                  ) : (
                    <span className="text-muted">None</span>
                  )}
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="4" className="text-center text-muted">No users found.</td>
            </tr>
          )}
        </tbody>
      </Table>
    </Container>
  );
};

export default Users;
