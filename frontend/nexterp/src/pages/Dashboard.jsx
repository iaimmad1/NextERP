import React from 'react';
import { Container, Card, Alert, Badge } from 'react-bootstrap';
import { getStoredUser } from '../services/api';

const Dashboard = () => {
  const user = getStoredUser();

  if (!user) {
    return (
      <Container className="mt-5">
        <Alert variant="warning">No user data found. Please log in again.</Alert>
      </Container>
    );
  }

  return (
    <Container className="mt-5">
      <h2>Dashboard</h2>
      <Alert variant="success">
        Welcome, {user.fullName || user.username}!
      </Alert>

      <Card className="shadow-sm mt-4">
        <Card.Header as="h5">Your Profile</Card.Header>
        <Card.Body>
          <Card.Title>{user.fullName}</Card.Title>
          <Card.Text>
            <strong>Username:</strong> {user.username} <br />
            <strong>Email:</strong> {user.email} <br />
            <strong>Tenant:</strong> {user.tenantName}
          </Card.Text>

          <div className="mb-3">
            <strong>Roles: </strong>
            {user.roles && user.roles.length > 0 ? (
              user.roles.map(role => (
                <Badge bg="info" className="me-1" key={role}>{role}</Badge>
              ))
            ) : (
              <span className="text-muted">None</span>
            )}
          </div>

          <div>
            <strong>Permissions: </strong>
            {user.permissions && user.permissions.length > 0 ? (
              user.permissions.map(perm => (
                <Badge bg="secondary" className="me-1 mb-1" key={perm}>{perm}</Badge>
              ))
            ) : (
              <span className="text-muted">None</span>
            )}
          </div>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Dashboard;

