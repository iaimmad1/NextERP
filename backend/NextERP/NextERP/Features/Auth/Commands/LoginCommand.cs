using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Enums;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;
using NextERP.Infrastructure.Data;
using BCrypt.Net;


namespace NextERP.Features.Auth.Commands
{
    public record LoginCommand(LoginRequest Request) : IRequest<ApiResponse<AuthResponse>>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginRequest = request.Request;

            // Find user by username or email
            var user = await _context.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Username == loginRequest.UsernameOrEmail || u.Email == loginRequest.UsernameOrEmail, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                return ApiResponse<AuthResponse>.Error("Invalid username or password", 401);

            if (user.Status != ENUM_UserStatus.Active)
                return ApiResponse<AuthResponse>.Error("Account is not active", 403);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = "web"; // You can get from HttpContext
            await _context.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var (accessToken, refreshToken, expiry) = await _jwtService.GenerateTokensAsync(user, cancellationToken);

            // Load roles and permissions for response
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Select(ur => ur.Role!.RoleName)
                .ToListAsync(cancellationToken);

            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Select(rp => rp.Permission!.PermissionName)
                .Distinct()
                .ToListAsync(cancellationToken);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiry,
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles,
                    Permissions = permissions,
                    TenantId = user.TenantId,
                    TenantName = user.Tenant?.CompanyName ?? string.Empty
                }
            };

            return ApiResponse<AuthResponse>.Ok(response, "Login successful");
        }
    }
}
