using System.Linq;
using Microsoft.EntityFrameworkCore;
using MediatR;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Permissions.Queries
{
    public record GetUserPermissionsQuery : IRequest<ApiResponse<IEnumerable<string>>>;

    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, ApiResponse<IEnumerable<string>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public GetUserPermissionsQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext context)
        {
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<string>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return ApiResponse<IEnumerable<string>>.Unauthorized();

            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role!.RolePermissions)
                .Select(rp => rp.Permission!.PermissionName)
                .Distinct()
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<string>>.Ok(permissions);
        }
    }
}
