using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Features.Orders.Commands;
using NextERP.Features.Orders.DTOs;
using NextERP.Features.Orders.Queries;

namespace NextERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.OrdersView)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderResponseDto>>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetOrdersQuery(page, pageSize));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.OrdersCreate)]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            var result = await _mediator.Send(new CreateOrderCommand(request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Policy = Permissions.OrdersUpdate)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] string status)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand(id, status));
            return StatusCode(result.StatusCode, result);
        }
    }
}
