using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Features.Permissions.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Permissions.Queries
{
    public record GetAllPermissionsQuery : IRequest<ApiResponse<IEnumerable<PermissionCategoryDto>>>;

    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, ApiResponse<IEnumerable<PermissionCategoryDto>>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllPermissionsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<PermissionCategoryDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _context.Permissions
                .OrderBy(p => p.PermissionCategory)
                .ThenBy(p => p.DisplayName)
                .ToListAsync(cancellationToken);

            var grouped = permissions
                .GroupBy(p => p.PermissionCategory)
                .Select(g => new PermissionCategoryDto
                {
                    Category = g.Key,
                    Permissions = g.Select(p => new PermissionDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName,
                        DisplayName = p.DisplayName,
                        Description = p.Description,
                        PermissionCategory = p.PermissionCategory
                    }).ToList()
                })
                .ToList();

            return ApiResponse<IEnumerable<PermissionCategoryDto>>.Ok(grouped);
        }
    }
}
