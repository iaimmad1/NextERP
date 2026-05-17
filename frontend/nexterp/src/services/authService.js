import { apiFetch, getTokens, clearTokens } from './api';

// ── Auth ─────────────────────────────────────────────────────────────────────

export const login = (credentials) =>
  apiFetch('/api/auth/login', { method: 'POST', body: JSON.stringify(credentials) });

export const register = (data) =>
  apiFetch('/api/auth/register', { method: 'POST', body: JSON.stringify(data) });

export const logout = async () => {
  const { refreshToken } = getTokens();
  if (refreshToken) {
    try {
      await apiFetch('/api/auth/logout', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: refreshToken,
      });
    } catch (_) { /* swallow – always clear storage */ }
  }
  clearTokens();
};

// ── Users ─────────────────────────────────────────────────────────────────────

export const getUsers = (page = 1, pageSize = 10) =>
  apiFetch(`/api/users?page=${page}&pageSize=${pageSize}`);

export const getUserById = (id) =>
  apiFetch(`/api/users/${id}`);

export const createUser = (data) =>
  apiFetch('/api/users', { method: 'POST', body: JSON.stringify(data) });

export const updateUser = (id, data) =>
  apiFetch(`/api/users/${id}`, { method: 'PUT', body: JSON.stringify(data) });

export const deleteUser = (id) =>
  apiFetch(`/api/users/${id}`, { method: 'DELETE' });

export const setUserStatus = (id, status) =>
  apiFetch(`/api/users/${id}/status`, { method: 'PATCH', body: JSON.stringify(status) });

export const assignRoleToUser = (userId, roleId) =>
  apiFetch(`/api/users/${userId}/roles/${roleId}`, { method: 'POST' });

export const removeRoleFromUser = (userId, roleId) =>
  apiFetch(`/api/users/${userId}/roles/${roleId}`, { method: 'DELETE' });

// ── Roles ─────────────────────────────────────────────────────────────────────

export const getRoles = () =>
  apiFetch('/api/roles');

export const getRoleById = (id) =>
  apiFetch(`/api/roles/${id}`);

export const createRole = (data) =>
  apiFetch('/api/roles', { method: 'POST', body: JSON.stringify(data) });

export const updateRole = (id, data) =>
  apiFetch(`/api/roles/${id}`, { method: 'PUT', body: JSON.stringify(data) });

export const deleteRole = (id) =>
  apiFetch(`/api/roles/${id}`, { method: 'DELETE' });

export const assignPermission = (roleId, permissionId) =>
  apiFetch(`/api/roles/${roleId}/permissions/${permissionId}`, { method: 'POST' });

export const revokePermission = (roleId, permissionId) =>
  apiFetch(`/api/roles/${roleId}/permissions/${permissionId}`, { method: 'DELETE' });

// ── Permissions ───────────────────────────────────────────────────────────────

export const getPermissions = () =>
  apiFetch('/api/permissions');

export const getMyPermissions = () =>
  apiFetch('/api/permissions/my-permissions');

// ── Products ──────────────────────────────────────────────────────────────────

export const getProducts = (page = 1, pageSize = 10, search = '') =>
  apiFetch(`/api/products?page=${page}&pageSize=${pageSize}&search=${search}`);

export const getProductById = (id) =>
  apiFetch(`/api/products/${id}`);

export const createProduct = (data) =>
  apiFetch('/api/products', { method: 'POST', body: JSON.stringify(data) });

export const updateProduct = (id, data) =>
  apiFetch(`/api/products/${id}`, { method: 'PUT', body: JSON.stringify(data) });

export const deleteProduct = (id) =>
  apiFetch(`/api/products/${id}`, { method: 'DELETE' });

// ── Orders ────────────────────────────────────────────────────────────────────

export const getOrders = (page = 1, pageSize = 10) =>
  apiFetch(`/api/orders?page=${page}&pageSize=${pageSize}`);

export const createOrder = (data) =>
  apiFetch('/api/orders', { method: 'POST', body: JSON.stringify(data) });

export const updateOrderStatus = (id, status) =>
  apiFetch(`/api/orders/${id}/status`, { method: 'PATCH', body: JSON.stringify(status) });

