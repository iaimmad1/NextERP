using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data;
using NextERP.Infrastructure.Security;

namespace NextERP.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;

        public JwtService(IOptions<JwtSettings> jwtSettings, ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            
            // Validate JWT settings
            if (string.IsNullOrWhiteSpace(_jwtSettings.Secret))
                throw new InvalidOperationException("JWT Secret is not configured. Please check appsettings.json");
            
            if (_jwtSettings.Secret.Length < 32)
                throw new InvalidOperationException("JWT Secret must be at least 32 characters long for security");
                
            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured");
                
            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
                throw new InvalidOperationException("JWT Audience is not configured");
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime Expiry)> GenerateTokensAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            // Revoke any existing refresh tokens for this user (optional, but good practice)
            var existingTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.UserId && !rt.IsRevoked);
            foreach (var token in existingTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            var refreshTokenEntity = new RefreshTokenModel
            {
                Token = refreshToken,
                UserId = user.UserId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "web" // You can get IP from HttpContext if needed
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return (accessToken, refreshToken, expiry);
        }

        private string GenerateAccessToken(UserModel user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user?.Email) || user?.UserId <= 0)
                    throw new ArgumentException("Invalid user data for token generation");
                    
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("tenant_id", user.TenantId.ToString()),
                new Claim("first_name", user.FirstName),
                new Claim("last_name", user.LastName)
            };

                // Load roles and permissions for this user
                var userRoles = _context.UserRoles
                    .Where(ur => ur.UserId == user.UserId)
                    .Select(ur => ur.Role!.RoleName)
                    .ToList();

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var permissions = _context.UserRoles
                    .Where(ur => ur.UserId == user.UserId)
                    .SelectMany(ur => ur.Role!.RolePermissions)
                    .Select(rp => rp.Permission!.PermissionName)
                    .Distinct()
                    .ToList();

                foreach (var perm in permissions)
                {
                    claims.Add(new Claim("permission", perm));
                }

                var secretBytes = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
                if (secretBytes.Length == 0)
                    throw new InvalidOperationException("JWT Secret is empty. Please verify appsettings configuration.");
                    
                var key = new SymmetricSecurityKey(secretBytes);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate access token: {ex.Message}", ex);
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId, CancellationToken cancellationToken = default)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId && !rt.IsRevoked, cancellationToken);

            return token != null && token.ExpiryDate > DateTime.UtcNow;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
