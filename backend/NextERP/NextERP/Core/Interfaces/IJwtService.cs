using System.Security.Claims;
using NextERP.Core.Entities;

namespace NextERP.Core.Interfaces
{
    public interface IJwtService
    {
        Task<(string AccessToken, string RefreshToken, DateTime Expiry)> GenerateTokensAsync(UserModel user, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
