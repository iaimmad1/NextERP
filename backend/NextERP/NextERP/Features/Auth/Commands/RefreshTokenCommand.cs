using System.Security.Claims;
using MediatR;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;

namespace NextERP.Features.Auth.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<ApiResponse<AuthResponse>>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponse>>
    {
        private readonly IJwtService _jwtService;
        private readonly IRepository<UserModel> _userRepository;

        public RefreshTokenCommandHandler(IJwtService jwtService, IRepository<UserModel> userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.Request.RefreshToken);
            if (principal == null)
                return ApiResponse<AuthResponse>.Error("Invalid token", 401);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return ApiResponse<AuthResponse>.Error("Invalid user ID in token", 401);

            var isValid = await _jwtService.ValidateRefreshTokenAsync(request.Request.RefreshToken, userId, cancellationToken);
            if (!isValid)
                return ApiResponse<AuthResponse>.Error("Invalid or expired refresh token", 401);

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return ApiResponse<AuthResponse>.Error("User not found", 404);

            // Revoke old refresh token
            await _jwtService.RevokeRefreshTokenAsync(request.Request.RefreshToken, cancellationToken);

            // Generate new tokens
            var (newAccessToken, newRefreshToken, expiry) = await _jwtService.GenerateTokensAsync(user, cancellationToken);

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiry,
                User = new UserDto { UserId = user.UserId } // Minimal user info
            };

            return ApiResponse<AuthResponse>.Ok(response, "Token refreshed");
        }
    }
}
