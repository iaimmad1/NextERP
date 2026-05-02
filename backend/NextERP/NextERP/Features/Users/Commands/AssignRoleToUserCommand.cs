using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class AssignRoleToUserCommand : IRequest<ApiResponse<bool>>, IAuthorizedRequest
    {
        public AssignRoleToUserCommand(int userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public int UserId { get; }
        public int RoleId { get; }
        public string? RequiredPermission => Common.Constants.Permissions.RolesAssign;
        public string? RequiredRole => null;
    }
}
