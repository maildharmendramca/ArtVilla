using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public CategoriesController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
    {
        var categories = await _categoryService.GetCategoryTreeAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories));
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetTree()
    {
        var categories = await _categoryService.GetCategoryTreeAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories));
    }

    [HttpGet("summaries")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategorySummaryDto>>>> GetSummaries()
    {
        var categories = await _categoryService.GetCategoryTreeAsync();
        var summaries = FlattenCategories(categories).Select(c => new CategorySummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug
        });
        return Ok(ApiResponse<IEnumerable<CategorySummaryDto>>.Ok(summaries));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetBySlug(string slug)
    {
        var category = await _categoryService.GetBySlugAsync(slug);
        if (category is null)
            return NotFound(ApiResponse<CategoryDto>.Fail("Category not found."));
        return Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    [HttpGet("{slug}/products")]
    public async Task<ActionResult<ApiResponse<List<ProductListDto>>>> GetCategoryProducts(
        string slug, [FromQuery] ProductFilterParams filters)
    {
        filters.CategorySlug = slug;
        var result = await _productService.GetProductsAsync(filters);
        return Ok(new ApiResponse<List<ProductListDto>>
        {
            Success = true,
            Data = result.Items,
            Pagination = result.Pagination
        });
    }

    private static IEnumerable<CategoryDto> FlattenCategories(IEnumerable<CategoryDto> categories)
    {
        foreach (var c in categories)
        {
            yield return c;
            foreach (var sub in FlattenCategories(c.SubCategories))
                yield return sub;
        }
    }
}
