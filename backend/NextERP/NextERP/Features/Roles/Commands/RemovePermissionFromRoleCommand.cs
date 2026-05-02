using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Roles.Commands
{
    public record RemovePermissionFromRoleCommand(int RoleId, int PermissionId)
        : IRequest<ApiResponse<bool>>, IAuthorizedRequest
    {
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesAssign;
        public string? RequiredRole => null;
    }

    public class RemovePermissionFromRoleCommandHandler : IRequestHandler<RemovePermissionFromRoleCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RemovePermissionFromRoleCommandHandler(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var role = await _context.Roles.FindAsync(new object[] { request.RoleId }, cancellationToken);
            if (role == null || role.TenantId != tenantId)
                return ApiResponse<bool>.NotFound("Role not found");

            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId, cancellationToken);

            if (rolePermission == null)
                return ApiResponse<bool>.NotFound("Permission not assigned to role");

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Permission removed from role successfully");
        }
    }
}