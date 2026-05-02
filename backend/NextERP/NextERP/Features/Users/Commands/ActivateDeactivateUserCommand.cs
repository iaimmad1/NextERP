using MediatR;
using NextERP.Common.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class ActivateDeactivateUserCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; }
        public string Status { get; }

        public ActivateDeactivateUserCommand(int id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
