import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, Alert, ListGroup, Spinner } from 'react-bootstrap';
import { useNavigate, Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useToast } from '../context/ToastContext';
import { createOrder } from '../services/authService';
import { getTokens, getStoredUser } from '../services/api';

const Checkout = () => {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const { cart, cartTotal, clearCart } = useCart();
  const { accessToken } = getTokens();
  const user = getStoredUser();

  const [form, setForm] = useState({
    shippingAddress: user?.address || '',
    paymentMethod: 'COD'
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!accessToken) {
      showToast('Please login to complete your order', 'info');
      navigate('/login?redirect=/checkout');
    }
    if (cart.length === 0) {
      navigate('/');
    }
  }, [accessToken, cart, navigate, showToast]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const orderData = {
        shippingAddress: form.shippingAddress,
        paymentMethod: form.paymentMethod,
        items: cart.map(item => ({
          productId: item.productId,
          quantity: item.quantity
        }))
      };

      const res = await createOrder(orderData);
      if (res.success) {
        showToast('Order placed successfully!', 'success');
        clearCart();
        navigate('/');
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      setError(err.message || 'Failed to place order');
    } finally {
      setLoading(false);
    }
  };

  if (!accessToken || cart.length === 0) return null;

  return (
    <Container className="mt-4 pb-5">
      <h3 className="mb-4 text-center">Checkout</h3>
      <Row className="g-4">
        <Col lg={7}>
          <Card className="shadow-sm border-0 p-4">
            <h5 className="mb-4">Shipping & Payment</h5>
            {error && <Alert variant="danger">{error}</Alert>}
            <Form onSubmit={handleSubmit}>
              <Form.Group className="mb-4">
                <Form.Label className="fw-semibold">Shipping Address</Form.Label>
                <Form.Control 
                  as="textarea" rows={3} required
                  placeholder="Enter your full address"
                  value={form.shippingAddress}
                  onChange={e => setForm({...form, shippingAddress: e.target.value})}
                />
              </Form.Group>

              <Form.Group className="mb-4">
                <Form.Label className="fw-semibold">Payment Method</Form.Label>
                <div className="d-flex gap-4">
                  <Form.Check 
                    type="radio" label="Cash on Delivery" name="payment" 
                    id="cod" checked={form.paymentMethod === 'COD'}
                    onChange={() => setForm({...form, paymentMethod: 'COD'})}
                  />
                  <Form.Check 
                    type="radio" label="Credit/Debit Card (Mock)" name="payment" 
                    id="card" checked={form.paymentMethod === 'Card'}
                    onChange={() => setForm({...form, paymentMethod: 'Card'})}
                  />
                </div>
              </Form.Group>

              <Button 
                type="submit" variant="primary" size="lg" 
                className="w-100 mt-3 py-3 fw-bold"
                disabled={loading}
              >
                {loading ? <><Spinner animation="border" size="sm" className="me-2" /> Placing Order...</> : 'Place Order'}
              </Button>
              <div className="text-center mt-3">
                <Link to="/cart" className="text-muted small text-decoration-none">← Return to Cart</Link>
              </div>
            </Form>
          </Card>
        </Col>

        <Col lg={5}>
          <Card className="shadow-sm border-0">
            <Card.Header className="bg-white py-3 fw-bold">Order Summary</Card.Header>
            <ListGroup variant="flush">
              {cart.map(item => (
                <ListGroup.Item key={item.productId} className="d-flex justify-content-between align-items-center py-3">
                  <div className="d-flex align-items-center">
                    <div style={{ width: '40px', height: '40px', backgroundColor: '#f8f9fa' }} className="rounded me-2 d-flex align-items-center justify-content-center">
                      {item.imageUrl ? <img src={item.imageUrl} alt="" style={{ maxWidth: '100%' }} /> : '📦'}
                    </div>
                    <div>
                      <div className="small fw-bold">{item.name}</div>
                      <div className="text-muted x-small">Qty: {item.quantity}</div>
                    </div>
                  </div>
                  <span className="small">${(item.price * item.quantity).toFixed(2)}</span>
                </ListGroup.Item>
              ))}
              <ListGroup.Item className="bg-light py-3">
                <div className="d-flex justify-content-between mb-2">
                  <span>Subtotal</span>
                  <span>${cartTotal.toFixed(2)}</span>
                </div>
                <div className="d-flex justify-content-between mb-2">
                  <span>Shipping</span>
                  <span className="text-success fw-bold">FREE</span>
                </div>
                <hr />
                <div className="d-flex justify-content-between h5 fw-bold mb-0">
                  <span>Total</span>
                  <span className="text-primary">${cartTotal.toFixed(2)}</span>
                </div>
              </ListGroup.Item>
            </ListGroup>
          </Card>
          <div className="mt-3 p-3 bg-light rounded border small text-muted">
             By placing this order, you agree to NextERP's Terms of Service and Privacy Policy.
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default Checkout;
