using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Orders.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Orders.Queries
{
    public record GetOrdersQuery(int Page = 1, int PageSize = 10)
        : IRequest<ApiResponse<PagedResponse<OrderResponseDto>>>;

    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResponse<PagedResponse<OrderResponseDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetOrdersQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResponse<OrderResponseDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<PagedResponse<OrderResponseDto>>.Unauthorized("Tenant context missing");

            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.TenantId == tenantId);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.OrderId,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    PaymentMethod = o.PaymentMethod,
                    CreatedAt = o.CreatedAt,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product!.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        LineTotal = i.LineTotal
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResponse<OrderResponseDto>(items, totalCount, request.Page, request.PageSize);
            return ApiResponse<PagedResponse<OrderResponseDto>>.Ok(pagedResponse);
        }
    }
}
