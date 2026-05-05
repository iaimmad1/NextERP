export const getTokens = () => {
  const accessToken = localStorage.getItem('accessToken');
  const refreshToken = localStorage.getItem('refreshToken');
  return { accessToken, refreshToken };
};

export const setTokens = (accessToken, refreshToken) => {
  localStorage.setItem('accessToken', accessToken);
  localStorage.setItem('refreshToken', refreshToken);
};

export const clearTokens = () => {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('currentUser');
};

export const storeUser = (user) => {
  localStorage.setItem('currentUser', JSON.stringify(user));
};

export const getStoredUser = () => {
  const raw = localStorage.getItem('currentUser');
  return raw ? JSON.parse(raw) : null;
};

let isRefreshing = false;
let refreshSubscribers = [];

const subscribeTokenRefresh = (cb) => {
  refreshSubscribers.push(cb);
};

const onRefreshed = (newAccessToken) => {
  refreshSubscribers.map(cb => cb(newAccessToken));
  refreshSubscribers = [];
};

export const apiFetch = async (url, options = {}) => {
  const { accessToken } = getTokens();

  const headers = new Headers(options.headers || {});
  headers.set('Content-Type', 'application/json');
  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`);
  }

  const config = {
    ...options,
    headers,
  };

  try {
    let response = await fetch(url, config);

    if (response.status === 401 && accessToken) {
      if (!isRefreshing) {
        isRefreshing = true;
        const { refreshToken } = getTokens();

        if (refreshToken) {
          try {
            const refreshResponse = await fetch('/api/auth/refresh', {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ refreshToken })
            });

            if (refreshResponse.ok) {
              const envelope = await refreshResponse.json();
              const newAccessToken = envelope.data.accessToken;
              const newRefreshToken = envelope.data.refreshToken;
              setTokens(newAccessToken, newRefreshToken);
              isRefreshing = false;
              onRefreshed(newAccessToken);

              // Retry original request with new token
              headers.set('Authorization', `Bearer ${newAccessToken}`);
              config.headers = headers;
              return await fetch(url, config);
            } else {
              throw new Error('Refresh failed');
            }
          } catch (e) {
            isRefreshing = false;
            clearTokens();
            refreshSubscribers = [];
            window.location.href = '/login';
            return Promise.reject(e);
          }
        } else {
          isRefreshing = false;
          clearTokens();
          window.location.href = '/login';
          return Promise.reject('No refresh token');
        }
      } else {
        // Wait for token refresh
        return new Promise(resolve => {
          subscribeTokenRefresh(newAccessToken => {
            headers.set('Authorization', `Bearer ${newAccessToken}`);
            config.headers = headers;
            resolve(fetch(url, config));
          });
        });
      }
    }

    let parsedData = null;
    const contentType = response.headers.get("content-type");
    if (contentType && contentType.indexOf("application/json") !== -1) {
      parsedData = await response.json();
    } else {
      parsedData = await response.text();
    }

    if (!response.ok) {
      const error = new Error(parsedData?.message || 'API Error');
      error.status = response.status;
      error.data = parsedData;
      throw error;
    }

    return parsedData;
  } catch (error) {
    throw error;
  }
};
