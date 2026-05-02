using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Roles.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Roles.Queries
{
    public record GetRoleByIdQuery(int Id) : IRequest<ApiResponse<RoleDetailDto>>;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDetailDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetRoleByIdQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<RoleDetailDto>.Unauthorized("Tenant context missing");

            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.RoleId == request.Id && r.TenantId == tenantId, cancellationToken);

            if (role == null)
                return ApiResponse<RoleDetailDto>.NotFound("Role not found");

            var dto = new RoleDetailDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                Status = role.Status,
                UserCount = role.UserRoles.Count,
                CreatedAt = role.CreatedAt,
                Permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    PermissionId = rp.Permission!.PermissionId,
                    PermissionName = rp.Permission.PermissionName,
                    DisplayName = rp.Permission.DisplayName,
                    Description = rp.Permission.Description,
                    PermissionCategory = rp.Permission.PermissionCategory,
                }).ToList()
            };

            return ApiResponse<RoleDetailDto>.Ok(dto);
        }
    }
}
