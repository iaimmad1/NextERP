using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Features.Permissions.DTOs;
using NextERP.Features.Permissions.Queries;

namespace NextERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all permissions (grouped by category)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Permissions.RolesAssign)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionCategoryDto>>>> GetAllPermissions()
        {
            var result = await _mediator.Send(new GetAllPermissionsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get permissions of the current user
        /// </summary>
        [HttpGet("my-permissions")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetMyPermissions()
        {
            var result = await _mediator.Send(new GetUserPermissionsQuery());
            return Ok(result);
        }
    }
}
