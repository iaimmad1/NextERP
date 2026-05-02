using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;
using NextERP.Features.Users.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Users.Queries
{
    public record GetUsersByTenantQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<ApiResponse<PagedResponse<UserResponseDto>>>;

    public class GetUsersByTenantQueryHandler : IRequestHandler<GetUsersByTenantQuery, ApiResponse<PagedResponse<UserResponseDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetUsersByTenantQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResponse<UserResponseDto>>> Handle(GetUsersByTenantQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<PagedResponse<UserResponseDto>>.Unauthorized("Tenant context missing");

            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.TenantId == tenantId);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(term) ||
                                         u.Email.ToLower().Contains(term) ||
                                         u.FirstName.ToLower().Contains(term) ||
                                         u.LastName.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Address = u.Address,
                    Status = u.Status,
                    Roles = u.UserRoles.Select(ur => ur.Role!.RoleName).ToList(),
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResponse<UserResponseDto>(items, totalCount, request.Page, request.PageSize);
            return ApiResponse<PagedResponse<UserResponseDto>>.Ok(pagedResponse);
        }
    }
}
