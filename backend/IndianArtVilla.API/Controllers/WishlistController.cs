using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> GetWishlist()
    {
        var items = await _wishlistService.GetWishlistAsync(UserId);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(items));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> AddToWishlist([FromBody] AddToWishlistDto dto)
    {
        await _wishlistService.AddToWishlistAsync(UserId, dto.ProductId);
        return Ok(ApiResponse<string>.Ok("Product added to wishlist."));
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult<ApiResponse<string>>> RemoveFromWishlist(int productId)
    {
        await _wishlistService.RemoveFromWishlistAsync(UserId, productId);
        return Ok(ApiResponse<string>.Ok("Product removed from wishlist."));
    }

    [HttpGet("check/{productId}")]
    public async Task<ActionResult<ApiResponse<WishlistCheckDto>>> CheckWishlist(int productId)
    {
        var exists = await _wishlistService.IsInWishlistAsync(UserId, productId);
        return Ok(ApiResponse<WishlistCheckDto>.Ok(new WishlistCheckDto { InWishlist = exists }));
    }
}
