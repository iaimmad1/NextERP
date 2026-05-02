using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Features.Permissions.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Roles.Queries
{
    public class GetAvailablePermissionsQuery : IRequest<ApiResponse<IEnumerable<NextERP.Features.Permissions.DTOs.PermissionDto>>>, IAuthorizedRequest
    {
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesAssign;
        public string? RequiredRole => null;
    }

    public class GetAvailablePermissionsQueryHandler : IRequestHandler<GetAvailablePermissionsQuery, ApiResponse<IEnumerable<NextERP.Features.Permissions.DTOs.PermissionDto>>>
    {
        private readonly ApplicationDbContext _context;

        public GetAvailablePermissionsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<NextERP.Features.Permissions.DTOs.PermissionDto>>> Handle(GetAvailablePermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _context.Permissions
                .Select(p => new NextERP.Features.Permissions.DTOs.PermissionDto
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    PermissionCategory = p.PermissionCategory
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<NextERP.Features.Permissions.DTOs.PermissionDto>>.Ok(permissions, "Permissions retrieved successfully");
        }
    }
}