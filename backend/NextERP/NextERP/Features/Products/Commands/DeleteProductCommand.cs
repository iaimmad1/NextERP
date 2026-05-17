using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Products.Commands
{
    public record DeleteProductCommand(int ProductId) : IRequest<ApiResponse<bool>>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteProductCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<bool>.Unauthorized("Tenant context missing");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && p.TenantId == tenantId, cancellationToken);

            if (product == null)
                return ApiResponse<bool>.NotFound("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Product deleted successfully");
        }
    }
}
