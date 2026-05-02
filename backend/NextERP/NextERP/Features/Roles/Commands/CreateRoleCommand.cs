using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Enums;
using NextERP.Core.Interfaces;

namespace NextERP.Features.Roles.Commands
{
    public record CreateRoleCommand(string Name, string? Description)
        : IRequest<ApiResponse<int>>, IAuthorizedRequest
    {
        public string? RequiredPermission => NextERP.Common.Constants.Permissions.RolesCreate;
        public string? RequiredRole => null;
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<int>>
    {
        private readonly IRepository<RoleModel> _roleRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateRoleCommandHandler(
            IRepository<RoleModel> roleRepository,
            ICurrentUserService currentUserService)
        {
            _roleRepository = roleRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<int>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<int>.Unauthorized("Tenant context missing");

            // Check if role with same name already exists for this tenant
            var exists = await _roleRepository.AnyAsync(r =>
                r.RoleName == request.Name && r.TenantId == tenantId, cancellationToken);

            if (exists)
                return ApiResponse<int>.Error($"Role '{request.Name}' already exists", 409);

            var role = new RoleModel
            {
                RoleName = request.Name,
                Description = request.Description,
                TenantId = tenantId.Value,
                IsSystemRole = false,
                Status = ENUM_UserStatus.Active
            };

            await _roleRepository.AddAsync(role, cancellationToken);
            return ApiResponse<int>.Ok(role.RoleId, "Role created successfully");
        }
    }
}
