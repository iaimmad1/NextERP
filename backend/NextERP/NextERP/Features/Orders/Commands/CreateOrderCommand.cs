using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Features.Orders.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Orders.Commands
{
    public record CreateOrderCommand(CreateOrderRequestDto Request) : IRequest<ApiResponse<OrderResponseDto>>;

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<OrderResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateOrderCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<OrderResponseDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<OrderResponseDto>.Unauthorized("Tenant context missing");

            var productIds = request.Request.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && p.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            if (products.Count != productIds.Distinct().Count())
                return ApiResponse<OrderResponseDto>.Error("Some products were not found or do not belong to this tenant");

            var order = new OrderModel
            {
                TenantId = tenantId.Value,
                CustomerId = _currentUserService.UserId,
                ShippingAddress = request.Request.ShippingAddress,
                PaymentMethod = request.Request.PaymentMethod,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            foreach (var itemReq in request.Request.Items)
            {
                var product = products.First(p => p.ProductId == itemReq.ProductId);
                var orderItem = new OrderItemModel
                {
                    ProductId = product.ProductId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = product.Price
                };
                order.Items.Add(orderItem);
                totalAmount += orderItem.LineTotal;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new OrderResponseDto
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Product " + i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            return ApiResponse<OrderResponseDto>.Created(response, "Order created successfully");
        }
    }
}
