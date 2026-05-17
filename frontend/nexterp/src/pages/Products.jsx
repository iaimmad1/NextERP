import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Card, Button, Badge, Spinner, Alert, InputGroup, Form, Pagination, Modal } from 'react-bootstrap';
import { getProducts, createProduct, updateProduct, deleteProduct } from '../services/authService';
import { getStoredUser } from '../services/api';
import { useToast } from '../context/ToastContext';
import ConfirmModal from '../components/ConfirmModal';

const PAGE_SIZE = 8;

const Products = () => {
  const { showToast } = useToast();
  const user = getStoredUser();
  const perms = new Set(user?.permissions || []);
  const isAdmin = user?.roles?.includes('Admin');
  const can = (p) => isAdmin || perms.has(p);

  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState('');

  // Modal states
  const [showModal, setShowModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [modalForm, setModalForm] = useState({
    name: '', sku: '', description: '', price: 0, stock: 0, imageUrl: '', category: ''
  });
  const [modalLoading, setModalLoading] = useState(false);
  const [modalError, setModalError] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(null);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    try {
      const res = await getProducts(page, PAGE_SIZE, search);
      if (res.success) {
        setProducts(res.data.items || []);
        setTotalCount(res.data.totalCount || 0);
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      setError(err.message || 'Failed to load products');
    } finally {
      setLoading(false);
    }
  }, [page, search]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  const handleOpenModal = (prod = null) => {
    if (prod) {
      setEditingProduct(prod);
      setModalForm({
        name: prod.name,
        sku: prod.sku,
        description: prod.description || '',
        price: prod.price,
        stock: prod.stock,
        imageUrl: prod.imageUrl || '',
        category: prod.category || ''
      });
    } else {
      setEditingProduct(null);
      setModalForm({ name: '', sku: '', description: '', price: 0, stock: 0, imageUrl: '', category: '' });
    }
    setModalError(null);
    setShowModal(true);
  };

  const handleModalSubmit = async (e) => {
    e.preventDefault();
    setModalLoading(true);
    setModalError(null);
    try {
      const res = editingProduct 
        ? await updateProduct(editingProduct.productId, modalForm)
        : await createProduct(modalForm);
      
      if (res.success) {
        showToast(editingProduct ? 'Product updated' : 'Product created');
        setShowModal(false);
        fetchProducts();
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      setModalError(err.message);
    } finally {
      setModalLoading(false);
    }
  };

  const handleDelete = async () => {
    try {
      const res = await deleteProduct(confirmDelete.productId);
      if (res.success) {
        showToast('Product deleted');
        setConfirmDelete(null);
        fetchProducts();
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      showToast(err.message, 'danger');
    }
  };

  const totalPages = Math.ceil(totalCount / PAGE_SIZE);

  return (
    <Container className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4>Products Management</h4>
        {can('products.create') && (
          <Button variant="primary" onClick={() => handleOpenModal()}>+ Add Product</Button>
        )}
      </div>

      <InputGroup className="mb-4" style={{ maxWidth: '400px' }}>
        <Form.Control
          placeholder="Search products..."
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1); }}
        />
        {search && <Button variant="outline-secondary" onClick={() => setSearch('')}>✕</Button>}
      </InputGroup>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center my-5"><Spinner animation="border" /></div>
      ) : (
        <>
          <Row className="g-4">
            {products.length === 0 ? (
              <Col className="text-center text-muted my-5">No products found.</Col>
            ) : products.map(prod => (
              <Col key={prod.productId} xs={12} sm={6} md={4} lg={3}>
                <Card className="h-100 shadow-sm border-0 product-card">
                  <div style={{ height: '200px', backgroundColor: '#f8f9fa', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    {prod.imageUrl ? (
                      <Card.Img variant="top" src={prod.imageUrl} style={{ maxHeight: '100%', width: 'auto' }} />
                    ) : (
                      <span className="text-muted">No Image</span>
                    )}
                  </div>
                  <Card.Body>
                    <div className="d-flex justify-content-between align-items-start mb-2">
                      <Card.Title className="h6 mb-0 text-truncate" title={prod.name}>{prod.name}</Card.Title>
                      <Badge bg={prod.stock > 10 ? 'success' : prod.stock > 0 ? 'warning' : 'danger'}>
                        {prod.stock > 0 ? `Stock: ${prod.stock}` : 'Out of Stock'}
                      </Badge>
                    </div>
                    <Card.Subtitle className="mb-2 text-muted small">SKU: {prod.sku}</Card.Subtitle>
                    <Card.Text className="text-primary fw-bold fs-5 mb-0">
                      ${prod.price.toFixed(2)}
                    </Card.Text>
                  </Card.Body>
                  {(can('products.edit') || can('products.delete')) && (
                    <Card.Footer className="bg-white border-top-0 d-flex gap-2 pb-3">
                      {can('products.edit') && (
                        <Button variant="outline-secondary" size="sm" className="flex-grow-1" onClick={() => handleOpenModal(prod)}>Edit</Button>
                      )}
                      {can('products.delete') && (
                        <Button variant="outline-danger" size="sm" onClick={() => setConfirmDelete(prod)}>Delete</Button>
                      )}
                    </Card.Footer>
                  )}
                </Card>
              </Col>
            ))}
          </Row>

          {totalPages > 1 && (
            <Pagination className="justify-content-center mt-5">
              <Pagination.Prev disabled={page === 1} onClick={() => setPage(p => p - 1)} />
              {[...Array(totalPages)].map((_, i) => (
                <Pagination.Item key={i + 1} active={page === i + 1} onClick={() => setPage(i + 1)}>
                  {i + 1}
                </Pagination.Item>
              ))}
              <Pagination.Next disabled={page === totalPages} onClick={() => setPage(p => p + 1)} />
            </Pagination>
          )}
        </>
      )}

      {/* Product Modal */}
      <Modal show={showModal} onHide={() => setShowModal(false)} size="lg" centered>
        <Form onSubmit={handleModalSubmit}>
          <Modal.Header closeButton>
            <Modal.Title>{editingProduct ? 'Edit Product' : 'Add New Product'}</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            {modalError && <Alert variant="danger">{modalError}</Alert>}
            <Row>
              <Col md={8}>
                <Form.Group className="mb-3">
                  <Form.Label>Product Name</Form.Label>
                  <Form.Control 
                    required 
                    value={modalForm.name} 
                    onChange={e => setModalForm({...modalForm, name: e.target.value})} 
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>SKU</Form.Label>
                  <Form.Control 
                    required 
                    value={modalForm.sku} 
                    onChange={e => setModalForm({...modalForm, sku: e.target.value})} 
                    disabled={!!editingProduct}
                  />
                </Form.Group>
              </Col>
            </Row>
            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control 
                as="textarea" rows={3} 
                value={modalForm.description} 
                onChange={e => setModalForm({...modalForm, description: e.target.value})} 
              />
            </Form.Group>
            <Row>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Price ($)</Form.Label>
                  <Form.Control 
                    type="number" step="0.01" min="0" required
                    value={modalForm.price} 
                    onChange={e => setModalForm({...modalForm, price: parseFloat(e.target.value)})} 
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Initial Stock</Form.Label>
                  <Form.Control 
                    type="number" min="0" required
                    value={modalForm.stock} 
                    onChange={e => setModalForm({...modalForm, stock: parseInt(e.target.value)})} 
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Category</Form.Label>
                  <Form.Control 
                    value={modalForm.category} 
                    onChange={e => setModalForm({...modalForm, category: e.target.value})} 
                  />
                </Form.Group>
              </Col>
            </Row>
            <Form.Group className="mb-3">
              <Form.Label>Image URL</Form.Label>
              <Form.Control 
                placeholder="https://example.com/image.jpg"
                value={modalForm.imageUrl} 
                onChange={e => setModalForm({...modalForm, imageUrl: e.target.value})} 
              />
            </Form.Group>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={() => setShowModal(false)}>Cancel</Button>
            <Button variant="primary" type="submit" disabled={modalLoading}>
              {modalLoading ? 'Saving...' : 'Save Product'}
            </Button>
          </Modal.Footer>
        </Form>
      </Modal>

      <ConfirmModal 
        show={!!confirmDelete} 
        onHide={() => setConfirmDelete(null)}
        onConfirm={handleDelete}
        title="Delete Product"
        message={`Are you sure you want to delete "${confirmDelete?.name}"?`}
      />
    </Container>
  );
};

export default Products;
