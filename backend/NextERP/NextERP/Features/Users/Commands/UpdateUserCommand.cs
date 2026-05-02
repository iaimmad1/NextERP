using MediatR;
using NextERP.Common.DTOs;
using NextERP.Features.Auth.DTOs;
using NextERP.Features.Users.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<ApiResponse<UserResponseDto>>
    {
        public int Id { get; }
        public UpdateUserRequestDto Request { get; }

        public UpdateUserCommand(int id, UpdateUserRequestDto request)
        {
            Id = id;
            Request = request;
        }
    }
}
