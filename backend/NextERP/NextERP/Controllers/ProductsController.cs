using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Features.Products.Commands;
using NextERP.Features.Products.DTOs;
using NextERP.Features.Products.Queries;

namespace NextERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.ProductsView)]
        public async Task<ActionResult<ApiResponse<PagedResponse<ProductResponseDto>>>> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetProductsQuery(page, pageSize, search));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.ProductsView)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProduct(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.ProductsCreate)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            var result = await _mediator.Send(new CreateProductCommand(request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.ProductsEdit)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
        {
            var result = await _mediator.Send(new UpdateProductCommand(id, request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.ProductsDelete)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
}
