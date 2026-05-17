using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Products.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Products.Queries
{
    public record GetProductByIdQuery(int ProductId) : IRequest<ApiResponse<ProductResponseDto>>;

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetProductByIdQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<ProductResponseDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<ProductResponseDto>.Unauthorized("Tenant context missing");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && p.TenantId == tenantId, cancellationToken);

            if (product == null)
                return ApiResponse<ProductResponseDto>.NotFound("Product not found");

            var dto = new ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };

            return ApiResponse<ProductResponseDto>.Ok(dto);
        }
    }
}
