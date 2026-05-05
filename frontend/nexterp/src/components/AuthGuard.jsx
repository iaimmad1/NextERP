import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { getTokens } from '../services/api';

const AuthGuard = () => {
  const { accessToken } = getTokens();

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};

export default AuthGuard;
