const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
  app.use(
    createProxyMiddleware({
      pathFilter: '/api',
      target: 'https://localhost:7129',
      changeOrigin: true,
      secure: false, // Required for self-signed localhost certificates
    })
  );
};


