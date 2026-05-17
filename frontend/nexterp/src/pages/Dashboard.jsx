import React, { useEffect, useState } from 'react';
import { Container, Card, Alert, Badge, Row, Col, Spinner } from 'react-bootstrap';
import { getStoredUser } from '../services/api';
import { getUsers, getRoles } from '../services/authService';

const StatCard = ({ title, value, icon, bg, loading }) => (
  <Card className={`text-white bg-${bg} shadow-sm h-100`}>
    <Card.Body className="d-flex align-items-center justify-content-between">
      <div>
        <div className="fs-1 fw-bold">{loading ? <Spinner animation="border" size="sm" /> : value}</div>
        <div className="opacity-75">{title}</div>
      </div>
      <div style={{ fontSize: '2.5rem', opacity: 0.4 }}>{icon}</div>
    </Card.Body>
  </Card>
);

const Dashboard = () => {
  const user = getStoredUser();
  const perms = new Set(user?.permissions || []);

  const [stats, setStats] = useState({ users: null, roles: null });
  const [loadingStats, setLoadingStats] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      setLoadingStats(true);
      try {
        const results = await Promise.allSettled([
          perms.has('users.view') ? getUsers(1, 1) : Promise.resolve(null),
          perms.has('roles.view') ? getRoles() : Promise.resolve(null),
        ]);
        const usersRes = results[0].value;
        const rolesRes = results[1].value;
        setStats({
          users: usersRes?.data?.totalCount ?? (usersRes?.data?.length ?? null),
          roles: rolesRes?.data?.length ?? null,
        });
      } catch (_) { /* stats are non-critical */ }
      finally { setLoadingStats(false); }
    };
    fetchStats();
  }, []); // eslint-disable-line

  if (!user) {
    return (
      <Container className="mt-5">
        <Alert variant="warning">No user session found. Please log in again.</Alert>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      {/* Welcome banner */}
      <Alert variant="success" className="d-flex align-items-center gap-2">
        <span style={{ fontSize: '1.4rem' }}>👋</span>
        <div>
          <strong>Welcome back, {user.fullName || user.username}!</strong>
          <span className="ms-2 text-muted small">{user.tenantName}</span>
        </div>
      </Alert>

      {/* Stat cards */}
      <Row className="g-3 mb-4">
        <Col xs={12} sm={6} lg={3}>
          <StatCard title="Total Users" value={stats.users ?? '—'} icon="👤" bg="primary" loading={loadingStats && perms.has('users.view')} />
        </Col>
        <Col xs={12} sm={6} lg={3}>
          <StatCard title="Roles" value={stats.roles ?? '—'} icon="🔐" bg="success" loading={loadingStats && perms.has('roles.view')} />
        </Col>
        <Col xs={12} sm={6} lg={3}>
          <StatCard title="Products" value="—" icon="📦" bg="warning" loading={false} />
        </Col>
        <Col xs={12} sm={6} lg={3}>
          <StatCard title="Orders" value="—" icon="🛒" bg="info" loading={false} />
        </Col>
      </Row>

      {/* Profile card */}
      <Row className="g-3">
        <Col lg={6}>
          <Card className="shadow-sm">
            <Card.Header as="h6" className="fw-semibold">Your Profile</Card.Header>
            <Card.Body>
              <table className="table table-sm table-borderless mb-0">
                <tbody>
                  <tr><td className="text-muted w-50">Username</td><td><strong>{user.username}</strong></td></tr>
                  <tr><td className="text-muted">Email</td><td>{user.email}</td></tr>
                  <tr><td className="text-muted">Tenant</td><td>{user.tenantName}</td></tr>
                  <tr>
                    <td className="text-muted">Roles</td>
                    <td>{user.roles?.map(r => <Badge bg="info" className="me-1" key={r}>{r}</Badge>)}</td>
                  </tr>
                </tbody>
              </table>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={6}>
          <Card className="shadow-sm">
            <Card.Header as="h6" className="fw-semibold">Your Permissions</Card.Header>
            <Card.Body style={{ maxHeight: 220, overflowY: 'auto' }}>
              <div className="d-flex flex-wrap gap-1">
                {user.permissions?.length > 0
                  ? user.permissions.map(p => <Badge bg="secondary" key={p}>{p}</Badge>)
                  : <span className="text-muted">None assigned</span>}
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Dashboard;
