using MediatR;
using NextERP.Common.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class RemoveRoleFromUserCommand : IRequest<ApiResponse<bool>>
    {
        public int UserId { get; }
        public int RoleId { get; }

        public RemoveRoleFromUserCommand(int userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
