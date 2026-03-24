using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetProductReviews(int productId)
    {
        var reviews = await _reviewService.GetProductReviewsAsync(productId);
        return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> AddReview([FromBody] CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var review = await _reviewService.AddReviewAsync(dto, userId);
        return CreatedAtAction(nameof(GetProductReviews),
            new { productId = dto.ProductId },
            ApiResponse<ReviewDto>.Ok(review, "Review submitted successfully."));
    }
}
