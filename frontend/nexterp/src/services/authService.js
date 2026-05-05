import { apiFetch, getTokens, clearTokens } from './api';

export const login = (credentials) => {
  return apiFetch('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials)
  });
};

export const register = (data) => {
  return apiFetch('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(data)
  });
};

export const logout = async () => {
  const { refreshToken } = getTokens();
  if (refreshToken) {
    try {
      await apiFetch('/api/auth/logout', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: refreshToken
      });
    } catch (error) {
      console.error('Logout error:', error);
    }
  }
  clearTokens();
};

export const getCurrentUser = () => {
  return apiFetch('/api/auth/current', {
    method: 'GET'
  });
};

export const getUsers = (page = 1, pageSize = 10) => {
  return apiFetch(`/api/users?page=${page}&pageSize=${pageSize}`, {
    method: 'GET'
  });
};
