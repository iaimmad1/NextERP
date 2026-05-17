import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Button, Badge, Spinner, Alert, InputGroup, Form } from 'react-bootstrap';
import { getProducts } from '../services/authService';
import { useCart } from '../context/CartContext';
import { useToast } from '../context/ToastContext';

const Catalog = () => {
  const { addToCart } = useCart();
  const { showToast } = useToast();
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [search, setSearch] = useState('');

  useEffect(() => {
    const fetch = async () => {
      try {
        const res = await getProducts(1, 20, search);
        if (res.success) setProducts(res.data.items || []);
        else throw new Error(res.message);
      } catch (err) {
        setError(err.message || 'Failed to load products');
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, [search]);

  const handleAddToCart = (p) => {
    addToCart(p);
    showToast(`${p.name} added to cart`);
  };

  return (
    <Container className="mt-4">
      <div className="text-center mb-5">
        <h2 className="fw-bold">Welcome to NextERP Shop</h2>
        <p className="text-muted">Quality products delivered to your doorstep</p>
      </div>

      <InputGroup className="mb-4 mx-auto" style={{ maxWidth: '600px' }}>
        <Form.Control 
          placeholder="Search for products..." 
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <Button variant="primary">Search</Button>
      </InputGroup>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center my-5"><Spinner animation="border" /></div>
      ) : (
        <Row className="g-4">
          {products.map(p => (
            <Col key={p.productId} xs={12} sm={6} md={4} lg={3}>
              <Card className="h-100 border-0 shadow-sm catalog-card hover-lift">
                <div style={{ height: '200px', backgroundColor: '#f8f9fa', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  {p.imageUrl ? (
                    <Card.Img variant="top" src={p.imageUrl} style={{ maxHeight: '100%', width: 'auto' }} />
                  ) : (
                    <span style={{ fontSize: '3rem' }}>📦</span>
                  )}
                </div>
                <Card.Body className="d-flex flex-column">
                  <Badge bg="light" text="dark" className="align-self-start mb-2 border">{p.category || 'General'}</Badge>
                  <Card.Title className="h6 mb-2">{p.name}</Card.Title>
                  <Card.Text className="text-muted small flex-grow-1 text-truncate-2">
                    {p.description || 'No description available.'}
                  </Card.Text>
                  <div className="d-flex justify-content-between align-items-center mt-3">
                    <span className="h5 mb-0 fw-bold">${p.price.toFixed(2)}</span>
                    <Button 
                      variant="primary" size="sm" 
                      onClick={() => handleAddToCart(p)}
                      disabled={p.stock <= 0}
                    >
                      {p.stock > 0 ? 'Add to Cart' : 'Out of Stock'}
                    </Button>
                  </div>
                </Card.Body>
              </Card>
            </Col>
          ))}
        </Row>
      )}
    </Container>
  );
};

export default Catalog;
