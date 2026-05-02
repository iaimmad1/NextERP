using MediatR;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;

namespace NextERP.Features.Auth.Commands
{
    public record LogoutCommand(string RefreshToken) : IRequest<ApiResponse<bool>>;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly IJwtService _jwtService;

        public LogoutCommandHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
            return ApiResponse<bool>.Ok(true, "Logged out successfully");
        }
    }
}
