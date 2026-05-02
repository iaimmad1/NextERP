using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Auth.Queries
{
    public record GetCurrentUserQuery : IRequest<ApiResponse<UserDto>>;

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ApiResponse<UserDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext context)
        {
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<ApiResponse<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return ApiResponse<UserDto>.Unauthorized("User not authenticated");

            var user = await _context.Users
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

            if (user == null)
                return ApiResponse<UserDto>.NotFound("User not found");

            var roles = user.UserRoles.Select(ur => ur.Role!.RoleName).ToList();
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Select(rp => rp.Permission!.PermissionName)
                .Distinct()
                .ToList();

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles,
                Permissions = permissions,
                TenantId = user.TenantId,
                TenantName = user.Tenant?.CompanyName ?? string.Empty
            };

            return ApiResponse<UserDto>.Ok(userDto, "Current user retrieved");
        }
    }
}
