using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Features.Auth.DTOs;
using NextERP.Features.Users.Commands;
using NextERP.Features.Users.DTOs;
using NextERP.Features.Users.Queries;

namespace NextERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all users in the current tenant
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Permissions.UsersView)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetUsersByTenantQuery(page, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.UsersView)]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUser(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Permissions.UsersCreate)]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateUser([FromBody] CreateUserRequestDto request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.UsersEdit)]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, request));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.UsersDelete)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        [HttpPost("{userId}/roles/{roleId}")]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<bool>>> AssignRole(int userId, int roleId)
        {
            var result = await _mediator.Send(new AssignRoleToUserCommand(userId, roleId));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Remove a role from a user
        /// </summary>
        [HttpDelete("{userId}/roles/{roleId}")]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveRole(int userId, int roleId)
        {
            var result = await _mediator.Send(new RemoveRoleFromUserCommand(userId, roleId));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Activate/Deactivate a user
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Policy = Permissions.UsersManage)]
        public async Task<ActionResult<ApiResponse<bool>>> SetUserStatus(int id, [FromBody] string status)
        {
            var result = await _mediator.Send(new ActivateDeactivateUserCommand(id, status));
            return StatusCode(result.StatusCode, result);
        }
    }
}
