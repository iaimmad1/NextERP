using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Orders.Commands
{
    public record UpdateOrderStatusCommand(int OrderId, string Status) : IRequest<ApiResponse<bool>>;

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateOrderStatusCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.TenantId == tenantId, cancellationToken);

            if (order == null)
                return ApiResponse<bool>.NotFound("Order not found");

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Order status updated successfully");
        }
    }
}
