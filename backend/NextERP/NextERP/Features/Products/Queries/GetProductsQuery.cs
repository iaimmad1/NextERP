using MediatR;
using Microsoft.EntityFrameworkCore;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;
using NextERP.Features.Products.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Products.Queries
{
    public record GetProductsQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<ApiResponse<PagedResponse<ProductResponseDto>>>;

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResponse<PagedResponse<ProductResponseDto>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetProductsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResponse<ProductResponseDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            if (!tenantId.HasValue)
                return ApiResponse<PagedResponse<ProductResponseDto>>.Unauthorized("Tenant context missing");

            var query = _context.Products
                .Where(p => p.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                         p.SKU.ToLower().Contains(term) ||
                                         (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    Category = p.Category,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResponse<ProductResponseDto>(items, totalCount, request.Page, request.PageSize);
            return ApiResponse<PagedResponse<ProductResponseDto>>.Ok(pagedResponse);
        }
    }
}
