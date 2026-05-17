using MediatR;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Features.Products.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Products.Commands
{
    public record CreateProductCommand(CreateProductRequestDto Request) : IRequest<ApiResponse<ProductResponseDto>>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateProductCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<ProductResponseDto>.Unauthorized("Tenant context missing");

            var product = new ProductModel
            {
                TenantId = tenantId.Value,
                Name = request.Request.Name,
                SKU = request.Request.SKU,
                Description = request.Request.Description,
                Price = request.Request.Price,
                Stock = request.Request.Stock,
                ImageUrl = request.Request.ImageUrl,
                Category = request.Request.Category,
                IsActive = true,
                CreatedBy = _currentUserService.UserId
            };

            _context.Products.Add(product);
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

            return ApiResponse<ProductResponseDto>.Created(response, "Product created successfully");
        }
    }
}
