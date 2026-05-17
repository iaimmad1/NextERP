import React from 'react';
import { Modal, Button } from 'react-bootstrap';

const ConfirmModal = ({ show, onHide, onConfirm, title = 'Confirm', message, confirmVariant = 'danger', confirmLabel = 'Delete', loading = false }) => (
  <Modal show={show} onHide={onHide} centered>
    <Modal.Header closeButton>
      <Modal.Title>{title}</Modal.Title>
    </Modal.Header>
    <Modal.Body>{message}</Modal.Body>
    <Modal.Footer>
      <Button variant="secondary" onClick={onHide} disabled={loading}>Cancel</Button>
      <Button variant={confirmVariant} onClick={onConfirm} disabled={loading}>
        {loading ? 'Processing...' : confirmLabel}
      </Button>
    </Modal.Footer>
  </Modal>
);

export default ConfirmModal;
