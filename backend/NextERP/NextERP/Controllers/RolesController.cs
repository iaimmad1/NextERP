using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Features.Roles.Commands;
using NextERP.Features.Roles.DTOs;
using NextERP.Features.Roles.Queries;

namespace NextERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all roles in the current tenant
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Permissions.RolesView)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleResponseDto>>>> GetRoles()
        {
            var result = await _mediator.Send(new GetRolesByTenantQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get role by ID with its permissions
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.RolesView)]
        public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRole(int id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Permissions.RolesCreate)]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateRole([FromBody] CreateRoleRequestDto request)
        {
            var result = await _mediator.Send(new CreateRoleCommand(request.Name, request.Description));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.RolesEdit)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateRole(int id, [FromBody] UpdateRoleRequestDto request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, request.Name, request.Description));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a role (cannot delete system roles)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.RolesDelete)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(int id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Assign a permission to a role
        /// </summary>
        [HttpPost("{roleId}/permissions/{permissionId}")]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<bool>>> AssignPermission(int roleId, int permissionId)
        {
            var result = await _mediator.Send(new AssignPermissionToRoleCommand(roleId, permissionId));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Remove a permission from a role
        /// </summary>
        [HttpDelete("{roleId}/permissions/{permissionId}")]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<bool>>> RemovePermission(int roleId, int permissionId)
        {
            var result = await _mediator.Send(new RemovePermissionFromRoleCommand(roleId, permissionId));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all available permissions (for assignment UI)
        /// </summary>
        [HttpGet("permissions")]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetAvailablePermissions()
        {
            var result = await _mediator.Send(new GetAvailablePermissionsQuery());
            return Ok(result);
        }
    }
}
