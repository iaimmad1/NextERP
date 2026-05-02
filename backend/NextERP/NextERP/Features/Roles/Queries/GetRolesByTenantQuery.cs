using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Roles.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Roles.Queries
{
    public record GetRolesByTenantQuery : IRequest<ApiResponse<IEnumerable<RoleResponseDto>>>;

    public class GetRolesByTenantQueryHandler : IRequestHandler<GetRolesByTenantQuery, ApiResponse<IEnumerable<RoleResponseDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetRolesByTenantQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> Handle(GetRolesByTenantQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<IEnumerable<RoleResponseDto>>.Unauthorized("Tenant context missing");

            var roles = await _context.Roles
                .Include(r => r.UserRoles)
                .Where(r => r.TenantId == tenantId)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    Status = r.Status,
                    UserCount = r.UserRoles.Count,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<RoleResponseDto>>.Ok(roles);
        }
    }
}
