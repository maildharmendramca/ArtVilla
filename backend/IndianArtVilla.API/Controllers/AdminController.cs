using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndianArtVilla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly IAuthService _authService;

    public AdminController(
        IDashboardService dashboardService,
        IProductService productService,
        ICategoryService categoryService,
        IOrderService orderService,
        IAuthService authService)
    {
        _dashboardService = dashboardService;
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
        _authService = authService;
    }

    // ── Dashboard ──────────────────────────────────────────────

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetDashboardStats()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }

    [HttpGet("dashboard/revenue-chart")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RevenueDataPoint>>>> GetRevenueChart(
        [FromQuery] int days = 30)
    {
        var data = await _dashboardService.GetRevenueChartAsync(days);
        return Ok(ApiResponse<IEnumerable<RevenueDataPoint>>.Ok(data));
    }

    [HttpGet("dashboard/top-products")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TopProductDto>>>> GetTopProducts(
        [FromQuery] int count = 10)
    {
        var data = await _dashboardService.GetTopProductsAsync(count);
        return Ok(ApiResponse<IEnumerable<TopProductDto>>.Ok(data));
    }

    // ── Products CRUD ──────────────────────────────────────────

    [HttpGet("products")]
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

    [HttpPost("products")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> CreateProduct(
        [FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProductById),
            new { id = product.Id },
            ApiResponse<ProductDetailDto>.Ok(product, "Product created."));
    }

    [HttpGet("products/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound(ApiResponse<ProductDetailDto>.Fail("Product not found."));

        return Ok(ApiResponse<ProductDetailDto>.Ok(product));
    }

    [HttpPut("products/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> UpdateProduct(
        int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productService.UpdateAsync(id, dto);
        return Ok(ApiResponse<ProductDetailDto>.Ok(product, "Product updated."));
    }

    [HttpDelete("products/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int id)
    {
        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Product deleted."));
    }

    // ── Categories CRUD ────────────────────────────────────────

    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        var categories = await _categoryService.GetCategoryTreeAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories));
    }

    [HttpPost("categories")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(
        [FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetCategories), null,
            ApiResponse<CategoryDto>.Ok(category, "Category created."));
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(
        int id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "Category updated."));
    }

    [HttpDelete("categories/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Category deleted."));
    }

    // ── Orders Management ──────────────────────────────────────

    [HttpGet("orders")]
    public async Task<ActionResult<ApiResponse<List<OrderListDto>>>> GetAllOrders(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? limit = null,
        [FromQuery] OrderStatus? status = null)
    {
        var effectivePageSize = limit ?? pageSize;
        var result = await _orderService.GetAllOrdersAsync(page, effectivePageSize, status);
        return Ok(new ApiResponse<List<OrderListDto>>
        {
            Success = true,
            Data = result.Items,
            Pagination = result.Pagination
        });
    }

    [HttpGet("orders/{id}")]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderDetail(int id)
    {
        var order = await _orderService.GetOrderDetailAdminAsync(id);
        if (order is null)
            return NotFound(ApiResponse<OrderDetailDto>.Fail("Order not found."));

        return Ok(ApiResponse<OrderDetailDto>.Ok(order));
    }

    [HttpPatch("orders/{id}/status")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateOrderStatus(
        int id, [FromBody] UpdateOrderStatusDto dto)
    {
        await _orderService.UpdateOrderStatusAsync(id, dto);
        return Ok(ApiResponse<string>.Ok("Order status updated."));
    }

    // ── Customers ──────────────────────────────────────────────

    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetCustomers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? limit = null)
    {
        var effectivePageSize = limit ?? pageSize;
        var result = await _authService.GetCustomersAsync(page, effectivePageSize);
        return Ok(new ApiResponse<List<CustomerDto>>
        {
            Success = true,
            Data = result.Items,
            Pagination = result.Pagination
        });
    }

    [HttpGet("customers/{id}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(string id)
    {
        var customer = await _authService.GetCustomerAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<CustomerDto>.Fail("Customer not found."));

        return Ok(ApiResponse<CustomerDto>.Ok(customer));
    }
}
