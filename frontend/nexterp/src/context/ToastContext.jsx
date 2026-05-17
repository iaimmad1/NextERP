import React, { createContext, useContext, useState, useCallback } from 'react';
import { Toast, ToastContainer } from 'react-bootstrap';

const ToastContext = createContext(null);

export const useToast = () => useContext(ToastContext);

let toastId = 0;

export const ToastProvider = ({ children }) => {
  const [toasts, setToasts] = useState([]);

  const showToast = useCallback((message, variant = 'success') => {
    const id = ++toastId;
    setToasts(prev => [...prev, { id, message, variant }]);
    setTimeout(() => setToasts(prev => prev.filter(t => t.id !== id)), 4000);
  }, []);

  const removeToast = (id) => setToasts(prev => prev.filter(t => t.id !== id));

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}
      <ToastContainer position="top-end" className="p-3" style={{ zIndex: 9999 }}>
        {toasts.map(({ id, message, variant }) => (
          <Toast
            key={id}
            bg={variant}
            onClose={() => removeToast(id)}
            show
            autohide
            delay={4000}
          >
            <Toast.Header closeButton>
              <strong className="me-auto">
                {variant === 'success' ? '✓ Success' : variant === 'danger' ? '✗ Error' : 'Info'}
              </strong>
            </Toast.Header>
            <Toast.Body className={variant === 'success' || variant === 'danger' ? 'text-white' : ''}>
              {message}
            </Toast.Body>
          </Toast>
        ))}
      </ToastContainer>
    </ToastContext.Provider>
  );
};
