using MediatR;
using NextERP.Common.DTOs;
using NextERP.Features.Auth.DTOs;
using NextERP.Features.Users.DTOs;

namespace NextERP.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<ApiResponse<UserResponseDto>>
    {
        public CreateUserRequestDto Request { get; }

        public CreateUserCommand(CreateUserRequestDto request)
        {
            Request = request;
        }
    }
}
