using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Products.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Products.Commands
{
    public record UpdateProductCommand(int ProductId, UpdateProductRequestDto Request) : IRequest<ApiResponse<ProductResponseDto>>;

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProductCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<ProductResponseDto>.Unauthorized("Tenant context missing");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && p.TenantId == tenantId, cancellationToken);

            if (product == null)
                return ApiResponse<ProductResponseDto>.NotFound("Product not found");

            if (request.Request.Name != null) product.Name = request.Request.Name;
            if (request.Request.Description != null) product.Description = request.Request.Description;
            if (request.Request.Price.HasValue) product.Price = request.Request.Price.Value;
            if (request.Request.Stock.HasValue) product.Stock = request.Request.Stock.Value;
            if (request.Request.ImageUrl != null) product.ImageUrl = request.Request.ImageUrl;
            if (request.Request.Category != null) product.Category = request.Request.Category;
            if (request.Request.IsActive.HasValue) product.IsActive = request.Request.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var response = new ProductResponseDto
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

            return ApiResponse<ProductResponseDto>.Ok(response, "Product updated successfully");
        }
    }
}
