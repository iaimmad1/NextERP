using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;

namespace NextERP.Features.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<ApiResponse<bool>>, IAuthorizedRequest
    {
        public int Id { get; }
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesDelete;
        public string? RequiredRole => null;

        public DeleteRoleCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
    {
        private readonly IRepository<RoleModel> _roleRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteRoleCommandHandler(
            IRepository<RoleModel> roleRepository,
            ICurrentUserService currentUserService)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role == null || role.TenantId != tenantId)
                return ApiResponse<bool>.NotFound("Role not found");

            // Prevent deletion of system roles
            if (role.IsSystemRole)
                return ApiResponse<bool>.Error("System roles cannot be deleted", 400);

            await _roleRepository.DeleteAsync(role, cancellationToken);
            return ApiResponse<bool>.Ok(true, "Role deleted successfully");
        }
    }
}