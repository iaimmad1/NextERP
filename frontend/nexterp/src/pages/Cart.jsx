import React from 'react';
import { Container, Table, Button, Row, Col, Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';

const Cart = () => {
  const { cart, removeFromCart, updateQuantity, cartTotal, clearCart } = useCart();

  if (cart.length === 0) {
    return (
      <Container className="mt-5 text-center">
        <div style={{ fontSize: '4rem' }}>🛒</div>
        <h3>Your cart is empty</h3>
        <p className="text-muted">Looks like you haven't added anything yet.</p>
        <Button as={Link} to="/" variant="primary">Start Shopping</Button>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      <h3 className="mb-4">Shopping Cart</h3>
      <Row>
        <Col lg={8}>
          <Card className="shadow-sm border-0">
            <Table responsive className="align-middle mb-0">
              <thead className="bg-light">
                <tr>
                  <th>Product</th>
                  <th>Price</th>
                  <th>Quantity</th>
                  <th>Total</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {cart.map(item => (
                  <tr key={item.productId}>
                    <td>
                      <div className="d-flex align-items-center">
                        <div style={{ width: '50px', height: '50px', backgroundColor: '#f8f9fa' }} className="rounded me-3 d-flex align-items-center justify-content-center">
                          {item.imageUrl ? <img src={item.imageUrl} alt="" style={{ maxWidth: '100%' }} /> : '📦'}
                        </div>
                        <div>
                          <div className="fw-bold">{item.name}</div>
                          <div className="text-muted small">SKU: {item.sku}</div>
                        </div>
                      </div>
                    </td>
                    <td>${item.price.toFixed(2)}</td>
                    <td>
                      <div className="d-flex align-items-center gap-2">
                        <Button size="sm" variant="outline-secondary" onClick={() => updateQuantity(item.productId, item.quantity - 1)}>-</Button>
                        <span>{item.quantity}</span>
                        <Button size="sm" variant="outline-secondary" onClick={() => updateQuantity(item.productId, item.quantity + 1)}>+</Button>
                      </div>
                    </td>
                    <td className="fw-bold">${(item.price * item.quantity).toFixed(2)}</td>
                    <td>
                      <Button variant="link" className="text-danger p-0" onClick={() => removeFromCart(item.productId)}>✕</Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Card>
          <Button variant="link" className="text-muted mt-2 p-0" onClick={clearCart}>Clear Cart</Button>
        </Col>
        <Col lg={4}>
          <Card className="shadow-sm border-0 p-3">
            <h5 className="mb-4">Order Summary</h5>
            <div className="d-flex justify-content-between mb-2">
              <span>Subtotal</span>
              <span>${cartTotal.toFixed(2)}</span>
            </div>
            <div className="d-flex justify-content-between mb-2">
              <span>Shipping</span>
              <span className="text-success">FREE</span>
            </div>
            <hr />
            <div className="d-flex justify-content-between mb-4 h4">
              <span>Total</span>
              <span className="fw-bold">${cartTotal.toFixed(2)}</span>
            </div>
            <Button as={Link} to="/checkout" variant="primary" size="lg" className="w-100">Proceed to Checkout</Button>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Cart;
