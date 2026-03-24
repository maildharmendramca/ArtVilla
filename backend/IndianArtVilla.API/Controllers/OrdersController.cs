using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> Checkout([FromBody] CheckoutRequestDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto, UserId);
        return CreatedAtAction(nameof(GetByOrderNumber),
            new { orderNumber = order.OrderNumber },
            ApiResponse<OrderDetailDto>.Ok(order, "Order placed successfully."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderListDto>>>> GetOrders(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? limit = null)
    {
        var effectivePageSize = limit ?? pageSize;
        var result = await _orderService.GetUserOrdersAsync(UserId, page, effectivePageSize);
        return Ok(new ApiResponse<List<OrderListDto>>
        {
            Success = true,
            Data = result.Items,
            Pagination = result.Pagination
        });
    }

    [HttpGet("{orderNumber}")]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetByOrderNumber(string orderNumber)
    {
        var order = await _orderService.GetOrderDetailAsync(orderNumber, UserId);
        if (order is null)
            return NotFound(ApiResponse<OrderDetailDto>.Fail("Order not found."));

        return Ok(ApiResponse<OrderDetailDto>.Ok(order));
    }

    [HttpPatch("{orderId}/cancel")]
    public async Task<ActionResult<ApiResponse<string>>> CancelOrder(int orderId)
    {
        var result = await _orderService.CancelOrderAsync(orderId, UserId);
        if (!result)
            return BadRequest(ApiResponse<string>.Fail("Order cannot be cancelled."));

        return Ok(ApiResponse<string>.Ok("Order cancelled successfully."));
    }
}
