import React, { useState, useEffect, useCallback } from 'react';
import { Container, Table, Button, Badge, Spinner, Alert, Pagination, Modal, Form } from 'react-bootstrap';
import { getOrders, updateOrderStatus } from '../services/authService';
import { useToast } from '../context/ToastContext';

const STATUSES = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

const Orders = () => {
  const { showToast } = useToast();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const [selectedOrder, setSelectedOrder] = useState(null);
  const [newStatus, setNewStatus] = useState('');
  const [statusLoading, setStatusLoading] = useState(false);

  const fetchOrders = useCallback(async () => {
    setLoading(true);
    try {
      const res = await getOrders(page);
      if (res.success) {
        setOrders(res.data.items || []);
        setTotalCount(res.data.totalCount || 0);
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      setError(err.message || 'Failed to load orders');
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  const handleUpdateStatus = async () => {
    setStatusLoading(true);
    try {
      const res = await updateOrderStatus(selectedOrder.orderId, newStatus);
      if (res.success) {
        showToast('Order status updated');
        setSelectedOrder(null);
        fetchOrders();
      } else {
        throw new Error(res.message);
      }
    } catch (err) {
      showToast(err.message, 'danger');
    } finally {
      setStatusLoading(false);
    }
  };

  const statusBadge = (status) => {
    switch (status) {
      case 'Pending': return <Badge bg="secondary">Pending</Badge>;
      case 'Processing': return <Badge bg="primary">Processing</Badge>;
      case 'Shipped': return <Badge bg="info">Shipped</Badge>;
      case 'Delivered': return <Badge bg="success">Delivered</Badge>;
      case 'Cancelled': return <Badge bg="danger">Cancelled</Badge>;
      default: return <Badge bg="light" text="dark">{status}</Badge>;
    }
  };

  const totalPages = Math.ceil(totalCount / 10);

  return (
    <Container className="mt-4">
      <h4 className="mb-4">Orders Management</h4>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center my-5"><Spinner animation="border" /></div>
      ) : (
        <>
          <Table striped hover responsive className="shadow-sm align-middle">
            <thead className="table-dark">
              <tr>
                <th>Order ID</th>
                <th>Date</th>
                <th>Total</th>
                <th>Payment</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {orders.length === 0 ? (
                <tr><td colSpan={6} className="text-center py-4">No orders found.</td></tr>
              ) : orders.map(order => (
                <tr key={order.orderId}>
                  <td>#{order.orderId}</td>
                  <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                  <td className="fw-bold">${order.totalAmount.toFixed(2)}</td>
                  <td>{order.paymentMethod}</td>
                  <td>{statusBadge(order.status)}</td>
                  <td>
                    <Button 
                      size="sm" variant="outline-primary" 
                      onClick={() => { setSelectedOrder(order); setNewStatus(order.status); }}
                    >
                      Update Status
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>

          {totalPages > 1 && (
            <Pagination className="justify-content-center mt-4">
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

      {/* Status Modal */}
      <Modal show={!!selectedOrder} onHide={() => setSelectedOrder(null)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Update Order Status #{selectedOrder?.orderId}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group>
            <Form.Label>New Status</Form.Label>
            <Form.Select value={newStatus} onChange={e => setNewStatus(e.target.value)}>
              {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
            </Form.Select>
          </Form.Group>
          {selectedOrder && (
            <div className="mt-3">
              <h6>Items:</h6>
              <ul className="small text-muted">
                {selectedOrder.items.map((item, idx) => (
                  <li key={idx}>{item.productName} x {item.quantity} (${item.lineTotal.toFixed(2)})</li>
                ))}
              </ul>
              <p className="small mb-0"><strong>Shipping to:</strong> {selectedOrder.shippingAddress}</p>
            </div>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setSelectedOrder(null)}>Cancel</Button>
          <Button variant="primary" onClick={handleUpdateStatus} disabled={statusLoading}>
            {statusLoading ? 'Updating...' : 'Save Changes'}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default Orders;
