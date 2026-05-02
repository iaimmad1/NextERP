using MediatR;
using NextERP.Common.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; }

        public DeleteUserCommand(int id)
        {
            Id = id;
        }
    }
}
