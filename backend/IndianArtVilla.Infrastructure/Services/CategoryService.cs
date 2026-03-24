using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync()
    {
        var allCategories = await _uow.Categories.Query()
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        return allCategories
            .Where(c => c.ParentCategoryId == null)
            .Select(c => MapToDto(c, allCategories));
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        var allCategories = await _uow.Categories.Query()
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .ToListAsync();

        var category = allCategories.FirstOrDefault(c => c.Slug == slug);
        return category == null ? null : MapToDto(category, allCategories);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Slug = Product.GenerateSlug(dto.Name),
            ParentCategoryId = dto.ParentCategoryId,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            SortOrder = dto.SortOrder,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            IsActive = true
        };

        _uow.Categories.Add(category);
        await _uow.SaveChangesAsync();

        var allCategories = await _uow.Categories.Query()
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .ToListAsync();

        return MapToDto(category, allCategories);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _uow.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException("Category", id);

        category.Name = dto.Name;
        category.Slug = Product.GenerateSlug(dto.Name);
        category.ParentCategoryId = dto.ParentCategoryId;
        category.Description = dto.Description;
        category.ImageUrl = dto.ImageUrl;
        category.SortOrder = dto.SortOrder;
        category.MetaTitle = dto.MetaTitle;
        category.MetaDescription = dto.MetaDescription;
        category.IsActive = dto.IsActive;

        await _uow.SaveChangesAsync();

        var allCategories = await _uow.Categories.Query()
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .ToListAsync();

        return MapToDto(category, allCategories);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _uow.Categories.Query()
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Category", id);

        if (category.SubCategories.Any(sc => sc.IsActive))
            throw new BusinessRuleViolationException("Cannot delete a category that has subcategories. Remove subcategories first.");

        var hasProducts = await _uow.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
            throw new BusinessRuleViolationException("Cannot delete a category that has products. Reassign products first.");

        category.IsActive = false;
        await _uow.SaveChangesAsync();
    }

    private static CategoryDto MapToDto(Category c, List<Category> allCategories) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        ImageUrl = c.ImageUrl,
        ParentCategoryId = c.ParentCategoryId,
        SortOrder = c.SortOrder,
        IsActive = c.IsActive,
        ProductCount = CountProducts(c.Id, allCategories),
        SubCategories = allCategories
            .Where(sc => sc.ParentCategoryId == c.Id && sc.IsActive)
            .OrderBy(sc => sc.SortOrder)
            .Select(sc => MapToDto(sc, allCategories))
            .ToList()
    };

    private static int CountProducts(int categoryId, List<Category> allCategories)
    {
        var category = allCategories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null) return 0;

        var count = category.Products.Count;
        foreach (var sub in allCategories.Where(c => c.ParentCategoryId == categoryId))
            count += CountProducts(sub.Id, allCategories);
        return count;
    }
}
