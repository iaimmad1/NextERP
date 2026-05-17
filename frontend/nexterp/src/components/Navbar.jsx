import React from 'react';
import { Navbar, Nav, Container, Button, Badge } from 'react-bootstrap';
import { NavLink, useNavigate } from 'react-router-dom';
import { logout } from '../services/authService';
import { getTokens, getStoredUser } from '../services/api';
import { useCart } from '../context/CartContext';

const AppNavbar = () => {
  const navigate = useNavigate();
  const { accessToken } = getTokens();
  const user = getStoredUser();
  const perms = new Set(user?.permissions || []);
  const isAdmin = user?.roles?.includes('Admin');
  const can = (p) => isAdmin || perms.has(p);
  const { cartCount } = useCart();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  // Helper: active link style for react-router NavLink
  const navLinkClass = ({ isActive }) =>
    `nav-link${isActive ? ' active fw-semibold' : ''}`;

  return (
    <Navbar bg="dark" variant="dark" expand="lg" className="shadow-sm">
      <Container>
        <Navbar.Brand as={NavLink} to="/" className="fw-bold fs-5">
          <span style={{ color: '#0d6efd' }}>Next</span>
          <span className="text-white">ERP</span>
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="main-nav" />

        <Navbar.Collapse id="main-nav">
          <Nav className="me-auto">
            <Nav.Link as={NavLink} to="/" end className={navLinkClass}>
              Home
            </Nav.Link>
            {accessToken && (
              <>
                <Nav.Link as={NavLink} to="/admin" className={navLinkClass}>
                  Admin
                </Nav.Link>

                {can('users.view') && (
                  <Nav.Link as={NavLink} to="/users" className={navLinkClass}>
                    Users
                  </Nav.Link>
                )}

                {can('roles.view') && (
                  <Nav.Link as={NavLink} to="/roles" className={navLinkClass}>
                    Roles
                  </Nav.Link>
                )}

                {can('products.view') && (
                  <Nav.Link as={NavLink} to="/products" className={navLinkClass}>
                    Products
                  </Nav.Link>
                )}

                {can('orders.view') && (
                  <Nav.Link as={NavLink} to="/orders" className={navLinkClass}>
                    Orders
                  </Nav.Link>
                )}
              </>
            )}
            <Nav.Link as={NavLink} to="/cart" className="position-relative me-2">
              <span style={{ fontSize: '1.2rem' }}>🛒</span>
              {cartCount > 0 && (
                <Badge 
                  pill bg="danger" 
                  className="position-absolute top-0 start-100 translate-middle"
                  style={{ fontSize: '0.6rem' }}
                >
                  {cartCount}
                </Badge>
              )}
            </Nav.Link>
          </Nav>

          <Nav className="align-items-center gap-2">
            {accessToken ? (
              <>
                {user && (
                  <Navbar.Text className="me-2">
                    <span className="text-white-50 small">Signed in as </span>
                    <span className="text-white fw-semibold">{user.username}</span>
                    {user.roles?.[0] && (
                      <Badge bg="secondary" className="ms-2" style={{ fontSize: '0.65rem' }}>
                        {user.roles[0]}
                      </Badge>
                    )}
                  </Navbar.Text>
                )}
                <Button variant="outline-light" size="sm" onClick={handleLogout}>
                  Logout
                </Button>
              </>
            ) : (
              <>
                <Nav.Link as={NavLink} to="/login" className={navLinkClass}>Login</Nav.Link>
                <Nav.Link as={NavLink} to="/register" className={navLinkClass}>Register</Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default AppNavbar;