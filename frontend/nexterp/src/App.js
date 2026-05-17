import React from 'react';
import { createBrowserRouter, RouterProvider, Outlet } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

import { ToastProvider } from './context/ToastContext';
import { CartProvider } from './context/CartContext';
import AppNavbar from './components/Navbar';
import AuthGuard from './components/AuthGuard';

import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';
import Roles from './pages/Roles';
import Products from './pages/Products';
import Orders from './pages/Orders';
import Catalog from './pages/Catalog';
import Cart from './pages/Cart';
import Checkout from './pages/Checkout';

// Layout: Navbar + page content
const Layout = () => (
  <>
    <AppNavbar />
    <div className="mt-2">
      <Outlet />
    </div>
  </>
);

const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      { index: true,        element: <Catalog /> },
      { path: 'dashboard',  element: <Dashboard /> },
      { path: 'login',      element: <Login /> },
      { path: 'register',   element: <Register /> },
      { path: 'cart',       element: <Cart /> },
      { path: 'checkout',   element: <Checkout /> },
      {
        // Protected routes – require a valid accessToken
        element: <AuthGuard />,
        children: [
          { path: 'admin',      element: <Dashboard /> },
          { path: 'users',      element: <Users /> },
          { path: 'roles',      element: <Roles /> },
          { path: 'products',   element: <Products /> },
          { path: 'orders',     element: <Orders /> },
        ],
      },
    ],
  },
]);

const App = () => (
  <ToastProvider>
    <CartProvider>
      <RouterProvider router={router} />
    </CartProvider>
  </ToastProvider>
);

export default App;