using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private string GetSessionId()
    {
        var sessionId = Request.Headers["X-Session-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Request.Cookies["cart_session"];
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("cart_session", sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
            }
        }
        return sessionId;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<CartDto>>> AddItem([FromBody] AddCartItemDto dto)
    {
        var cart = await _cartService.AddItemAsync(dto, GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart, "Item added to cart."));
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> UpdateItem(int cartItemId, [FromBody] UpdateQuantityRequest request)
    {
        var cart = await _cartService.UpdateItemAsync(cartItemId, request.Quantity, GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart, "Cart updated."));
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveItem(int cartItemId)
    {
        var cart = await _cartService.RemoveItemAsync(cartItemId, GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart, "Item removed from cart."));
    }

    [HttpPost("coupon")]
    public async Task<ActionResult<ApiResponse<CartDto>>> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        var cart = await _cartService.ApplyCouponAsync(request.Code, GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart, "Coupon applied."));
    }

    [HttpDelete("coupon")]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveCoupon()
    {
        var cart = await _cartService.RemoveCouponAsync(GetUserId(), GetSessionId());
        return Ok(ApiResponse<CartDto>.Ok(cart, "Coupon removed."));
    }

    [HttpPost("validate")]
    public Task<ActionResult<ApiResponse<object>>> ValidateCart([FromBody] object items)
    {
        return Task.FromResult<ActionResult<ApiResponse<object>>>(
            Ok(ApiResponse<object>.Ok(new { valid = true })));
    }

    [HttpPost("merge")]
    public async Task<ActionResult<ApiResponse<string>>> MergeCart()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(ApiResponse<string>.Fail("User must be authenticated to merge cart."));

        await _cartService.MergeGuestCartAsync(GetSessionId(), userId);
        return Ok(ApiResponse<string>.Ok("Cart merged successfully."));
    }
}
