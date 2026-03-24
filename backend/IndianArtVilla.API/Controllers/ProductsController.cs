using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;

    public ProductsController(IProductService productService, IReviewService reviewService)
    {
        _productService = productService;
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductListDto>>>> GetProducts(
        [FromQuery] ProductFilterParams filters)
    {
        var result = await _productService.GetProductsAsync(filters);
        return Ok(new ApiResponse<List<ProductListDto>>
        {
            Success = true,
            Data = result.Items,
            Pagination = result.Pagination
        });
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetBySlug(string slug)
    {
        var product = await _productService.GetBySlugAsync(slug);
        if (product is null)
            return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found."));

        return Ok(ApiResponse<ProductDetailDto>.Ok(product));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> GetFeatured(
        [FromQuery] int count = 8)
    {
        var products = await _productService.GetFeaturedAsync(count);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(products));
    }

    [HttpGet("new-arrivals")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> GetNewArrivals(
        [FromQuery] int count = 8)
    {
        var products = await _productService.GetNewArrivalsAsync(count);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(products));
    }

    [HttpGet("best-sellers")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> GetBestSellers(
        [FromQuery] int count = 8)
    {
        var products = await _productService.GetBestSellersAsync(count);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(products));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> Search(
        [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var products = await _productService.SearchAsync(q, page, pageSize);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(products));
    }

    [HttpGet("{productId:int}/related")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductListDto>>>> GetRelatedProducts(int productId)
    {
        var related = await _productService.GetRelatedProductsAsync(productId);
        return Ok(ApiResponse<IEnumerable<ProductListDto>>.Ok(related));
    }

    [HttpGet("{productId:int}/reviews")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetProductReviews(int productId)
    {
        var reviews = await _reviewService.GetProductReviewsAsync(productId);
        return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews));
    }
}
