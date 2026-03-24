using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync();
    Task<CategoryDto?> GetBySlugAsync(string slug);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
    Task DeleteAsync(int id);
}
