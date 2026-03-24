using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListDto>> GetProductsAsync(ProductFilterParams filters);
    Task<ProductDetailDto?> GetBySlugAsync(string slug);
    Task<IEnumerable<ProductListDto>> GetFeaturedAsync(int count = 8);
    Task<IEnumerable<ProductListDto>> GetNewArrivalsAsync(int count = 8);
    Task<IEnumerable<ProductListDto>> GetBestSellersAsync(int count = 8);
    Task<IEnumerable<ProductListDto>> SearchAsync(string query, int page, int pageSize);
    Task<IEnumerable<ProductListDto>> GetRelatedProductsAsync(int productId, int count = 8);
    Task<ProductDetailDto> CreateAsync(CreateProductDto dto);
    Task<ProductDetailDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}
