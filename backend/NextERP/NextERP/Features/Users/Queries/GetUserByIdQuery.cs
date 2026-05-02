using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;
using NextERP.Features.Users.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Users.Queries
{
    public record GetUserByIdQuery(int UserId) : IRequest<ApiResponse<UserResponseDto>>;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetUserByIdQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<UserResponseDto>.Unauthorized("Tenant context missing");

            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.TenantId == tenantId, cancellationToken);

            if (user == null)
                return ApiResponse<UserResponseDto>.NotFound("User not found");

            var dto = new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                Status = user.Status,
                Roles = user.UserRoles.Select(ur => ur.Role!.RoleName).ToList(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return ApiResponse<UserResponseDto>.Ok(dto);
        }
    }
}
