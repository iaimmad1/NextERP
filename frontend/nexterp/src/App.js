import React from 'react';
import { createBrowserRouter, RouterProvider, Outlet } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

import AppNavbar from './components/Navbar';
import AuthGuard from './components/AuthGuard';

import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';

// Layout component to include Navbar on all pages
const Layout = () => (
  <>
    <AppNavbar />
    <div className="mt-3">
      <Outlet />
    </div>
  </>
);

const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      {
        path: "login",
        element: <Login />
      },
      {
        path: "register",
        element: <Register />
      },
      {
        // Protected routes
        element: <AuthGuard />,
        children: [
          {
            index: true,
            element: <Dashboard />
          },
          {
            path: "users",
            element: <Users />
          }
        ]
      }
    ]
  }
]);

const App = () => {
  return <RouterProvider router={router} />;
};

export default App;