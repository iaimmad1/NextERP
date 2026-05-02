using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;

namespace NextERP.Features.Roles.Commands
{
    public record UpdateRoleCommand(int Id, string Name, string? Description)
        : IRequest<ApiResponse<bool>>, IAuthorizedRequest
    {
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesEdit;
        public string? RequiredRole => null;
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<bool>>
    {
        private readonly IRepository<RoleModel> _roleRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateRoleCommandHandler(
            IRepository<RoleModel> roleRepository,
            ICurrentUserService currentUserService)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role == null || role.TenantId != tenantId)
                return ApiResponse<bool>.NotFound("Role not found");

            // Prevent system roles from being renamed (optional)
            if (role.IsSystemRole && role.RoleName != request.Name)
                return ApiResponse<bool>.Error("System role name cannot be changed", 400);

            // Check if new name conflicts with another role in the same tenant
            if (role.RoleName != request.Name)
            {
                var exists = await _roleRepository.AnyAsync(r =>
                    r.RoleName == request.Name && r.TenantId == tenantId && r.RoleId != request.Id, cancellationToken);

                if (exists)
                    return ApiResponse<bool>.Error($"Role '{request.Name}' already exists", 409);
            }

            role.RoleName = request.Name;
            role.Description = request.Description;

            await _roleRepository.UpdateAsync(role, cancellationToken);
            return ApiResponse<bool>.Ok(true, "Role updated successfully");
        }
    }
}
