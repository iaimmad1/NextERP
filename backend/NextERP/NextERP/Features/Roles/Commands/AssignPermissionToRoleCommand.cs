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
    public record AssignPermissionToRoleCommand(int RoleId, int PermissionId)
        : IRequest<ApiResponse<bool>>, IAuthorizedRequest
    {
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesAssign;
        public string? RequiredRole => null;
    }

    public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AssignPermissionToRoleCommandHandler(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var role = await _context.Roles.FindAsync(new object[] { request.RoleId }, cancellationToken);
            if (role == null || role.TenantId != tenantId)
                return ApiResponse<bool>.NotFound("Role not found");

            var permission = await _context.Permissions.FindAsync(new object[] { request.PermissionId }, cancellationToken);
            if (permission == null)
                return ApiResponse<bool>.NotFound("Permission not found");

            // Check if the role already has this permission
            var existing = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId, cancellationToken);

            if (existing)
                return ApiResponse<bool>.Error("Permission already assigned to role", 400);

            var rolePermission = new RolePermissionModel
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            };

            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Permission assigned to role successfully");
        }
    }
}