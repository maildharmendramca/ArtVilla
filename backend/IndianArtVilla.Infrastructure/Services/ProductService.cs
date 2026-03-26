using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<ProductListDto>> GetProductsAsync(ProductFilterParams filters)
    {
        var query = _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(filters.CategorySlug))
        {
            var categoryIds = await GetCategoryIdsAsync(filters.CategorySlug);
            query = query.Where(p => categoryIds.Contains(p.CategoryId));
        }

        if (!string.IsNullOrEmpty(filters.Material))
            query = query.Where(p => p.Material != null && p.Material.ToLower() == filters.Material.ToLower());

        if (!string.IsNullOrEmpty(filters.Finish))
            query = query.Where(p => p.Finish != null && p.Finish.ToLower() == filters.Finish.ToLower());

        if (filters.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filters.MinPrice.Value);

        if (filters.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filters.MaxPrice.Value);

        if (filters.MinRating.HasValue)
            query = query.Where(p => p.Reviews.Any() && p.Reviews.Average(r => r.Rating) >= filters.MinRating.Value);

        if (filters.InStockOnly == true)
            query = query.Where(p => p.StockQuantity > 0);

        if (!string.IsNullOrEmpty(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search) ||
                (p.Material != null && p.Material.ToLower().Contains(search)));
        }

        var sorted = filters.SortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "rating" => query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "bestselling" => query.OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity)),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await sorted.CountAsync();
        var items = await sorted
            .Skip((filters.Page - 1) * filters.EffectivePageSize)
            .Take(filters.EffectivePageSize)
            .Select(p => MapToListDto(p))
            .ToListAsync();

        return new PagedResult<ProductListDto>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                CurrentPage = filters.Page,
                ItemsPerPage = filters.EffectivePageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filters.EffectivePageSize)
            }
        };
    }

    public async Task<ProductDetailDto?> GetBySlugAsync(string slug)
    {
        var product = await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        return product == null ? null : MapToDetailDto(product);
    }

    public async Task<ProductDetailDto?> GetByIdAsync(int id)
    {
        var product = await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null ? null : MapToDetailDto(product);
    }

    public async Task<IEnumerable<ProductListDto>> GetFeaturedAsync(int count = 8)
    {
        return await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(p => MapToListDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductListDto>> GetNewArrivalsAsync(int count = 8)
    {
        return await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(p => MapToListDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductListDto>> GetBestSellersAsync(int count = 8)
    {
        return await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
            .Take(count)
            .Select(p => MapToListDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductListDto>> SearchAsync(string query, int page, int pageSize)
    {
        var search = query.ToLower();
        return await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive &&
                (p.Name.ToLower().Contains(search) ||
                 p.Description.ToLower().Contains(search) ||
                 p.SKU.ToLower().Contains(search) ||
                 (p.Material != null && p.Material.ToLower().Contains(search))))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => MapToListDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductListDto>> GetRelatedProductsAsync(int productId, int count = 8)
    {
        var product = await _uow.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
        if (product is null)
            throw new NotFoundException("Product", productId);

        return await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.Id != productId)
            .Take(count)
            .Select(p => MapToListDto(p))
            .ToListAsync();
    }

    public async Task<ProductDetailDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Slug = Product.GenerateSlug(dto.Name),
            SKU = dto.SKU,
            Description = dto.Description,
            ShortDescription = dto.ShortDescription,
            Price = dto.Price,
            CompareAtPrice = dto.CompareAtPrice,
            CostPrice = dto.CostPrice,
            StockQuantity = dto.StockQuantity,
            TrackInventory = dto.TrackInventory,
            WeightGrams = dto.WeightGrams,
            Dimensions = dto.Dimensions,
            Material = dto.Material,
            Finish = dto.Finish,
            Capacity = dto.Capacity,
            IsFeatured = dto.IsFeatured,
            IsGiftable = dto.IsGiftable,
            IsCustomizable = dto.IsCustomizable,
            CategoryId = dto.CategoryId,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.Tags?.Any() == true)
        {
            product.Tags = dto.Tags.Select(t => new ProductTag { Tag = t }).ToList();
        }

        if (dto.Variants?.Any() == true)
        {
            product.Variants = dto.Variants.Select(v => new ProductVariant
            {
                Name = v.Name,
                SKU = v.SKU,
                PriceAdjustment = v.PriceAdjustment,
                StockQuantity = v.StockQuantity
            }).ToList();
        }
        else
        {
            // Auto-create a default variant so the product can be added to cart
            product.Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    Name = "Default",
                    SKU = dto.SKU,
                    PriceAdjustment = 0,
                    StockQuantity = dto.StockQuantity
                }
            };
        }

        if (dto.Images?.Any() == true)
        {
            product.Images = dto.Images.Select((img, i) => new ProductImage
            {
                ImageUrl = img.Url,
                AltText = img.Alt ?? dto.Name,
                IsPrimary = img.IsPrimary,
                SortOrder = img.SortOrder > 0 ? img.SortOrder : i + 1
            }).ToList();
        }

        _uow.Products.Add(product);
        await _uow.SaveChangesAsync();

        var saved = await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstAsync(p => p.Id == product.Id);

        return MapToDetailDto(saved);
    }

    public async Task<ProductDetailDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _uow.Products.Query()
            .Include(p => p.Tags)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new NotFoundException("Product", id);

        product.Name = dto.Name;
        product.Slug = Product.GenerateSlug(dto.Name);
        product.SKU = dto.SKU;
        product.Description = dto.Description;
        product.ShortDescription = dto.ShortDescription;
        product.Price = dto.Price;
        product.CompareAtPrice = dto.CompareAtPrice;
        product.CostPrice = dto.CostPrice;
        product.StockQuantity = dto.StockQuantity;
        product.TrackInventory = dto.TrackInventory;
        product.WeightGrams = dto.WeightGrams;
        product.Dimensions = dto.Dimensions;
        product.Material = dto.Material;
        product.Finish = dto.Finish;
        product.Capacity = dto.Capacity;
        product.IsFeatured = dto.IsFeatured;
        product.IsGiftable = dto.IsGiftable;
        product.IsCustomizable = dto.IsCustomizable;
        product.CategoryId = dto.CategoryId;
        product.MetaTitle = dto.MetaTitle;
        product.MetaDescription = dto.MetaDescription;
        product.IsActive = dto.IsActive;

        _uow.ProductTags.RemoveRange(product.Tags);
        if (dto.Tags?.Any() == true)
        {
            product.Tags = dto.Tags.Select(t => new ProductTag { ProductId = id, Tag = t }).ToList();
        }

        _uow.ProductVariants.RemoveRange(product.Variants);
        if (dto.Variants?.Any() == true)
        {
            product.Variants = dto.Variants.Select(v => new ProductVariant
            {
                ProductId = id,
                Name = v.Name,
                SKU = v.SKU,
                PriceAdjustment = v.PriceAdjustment,
                StockQuantity = v.StockQuantity
            }).ToList();
        }
        else
        {
            // Auto-create a default variant so the product can be added to cart
            product.Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = id,
                    Name = "Default",
                    SKU = dto.SKU,
                    PriceAdjustment = 0,
                    StockQuantity = dto.StockQuantity
                }
            };
        }

        if (dto.Images != null)
        {
            _uow.ProductImages.RemoveRange(product.Images);
            product.Images = dto.Images.Select((img, i) => new ProductImage
            {
                ProductId = id,
                ImageUrl = img.Url,
                AltText = img.Alt ?? dto.Name,
                IsPrimary = img.IsPrimary,
                SortOrder = img.SortOrder > 0 ? img.SortOrder : i + 1
            }).ToList();
        }

        await _uow.SaveChangesAsync();

        var updated = await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstAsync(p => p.Id == id);

        return MapToDetailDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id)
            ?? throw new NotFoundException("Product", id);
        product.IsActive = false;
        await _uow.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<List<int>> GetCategoryIdsAsync(string slug)
    {
        var allCategories = await _uow.Categories.Query().ToListAsync();
        var category = allCategories.FirstOrDefault(c => c.Slug == slug);

        if (category == null) return new List<int>();

        var ids = new List<int> { category.Id };
        CollectChildIds(category.Id, allCategories, ids);
        return ids;
    }

    private static void CollectChildIds(int parentId, List<Category> allCategories, List<int> ids)
    {
        foreach (var child in allCategories.Where(c => c.ParentCategoryId == parentId))
        {
            ids.Add(child.Id);
            CollectChildIds(child.Id, allCategories, ids);
        }
    }

    internal static ProductListDto MapToListDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        SKU = p.SKU,
        Price = p.Price,
        CompareAtPrice = p.CompareAtPrice,
        ShortDescription = p.ShortDescription,
        Material = p.Material,
        PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                          ?? p.Images.FirstOrDefault()?.ImageUrl,
        AverageRating = p.Reviews.Any() ? Math.Round((decimal)p.Reviews.Average(r => r.Rating), 1) : 0,
        ReviewCount = p.Reviews.Count,
        StockQuantity = p.StockQuantity,
        IsFeatured = p.IsFeatured,
        IsGiftable = p.IsGiftable,
        IsNew = p.CreatedAt >= DateTime.UtcNow.AddDays(-30),
        CategoryName = p.Category.Name
    };

    private static ProductDetailDto MapToDetailDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        SKU = p.SKU,
        Price = p.Price,
        CompareAtPrice = p.CompareAtPrice,
        Description = p.Description,
        ShortDescription = p.ShortDescription,
        Material = p.Material,
        Finish = p.Finish,
        Capacity = p.Capacity,
        Dimensions = p.Dimensions,
        WeightGrams = p.WeightGrams,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        IsFeatured = p.IsFeatured,
        IsGiftable = p.IsGiftable,
        IsCustomizable = p.IsCustomizable,
        AverageRating = p.Reviews.Any() ? Math.Round((decimal)p.Reviews.Average(r => r.Rating), 1) : 0,
        ReviewCount = p.Reviews.Count,
        CategoryDisplayName = p.Category.Name,
        CategoryIdValue = p.CategoryId,
        Category = new CategorySummaryDto
        {
            Id = p.Category.Id,
            Name = p.Category.Name,
            Slug = p.Category.Slug
        },
        Images = p.Images.Select(i => new ProductImageDto
        {
            Id = i.Id,
            Url = i.ImageUrl,
            AltText = i.AltText,
            IsPrimary = i.IsPrimary,
            SortOrder = i.SortOrder
        }).ToList(),
        Variants = p.Variants.Any()
            ? p.Variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                Name = v.Name,
                SKU = v.SKU,
                Price = p.Price + v.PriceAdjustment,
                PriceAdjustment = v.PriceAdjustment,
                CompareAtPrice = p.CompareAtPrice,
                StockQuantity = v.StockQuantity,
                ImageUrl = v.ImageUrl
            }).ToList()
            : new List<ProductVariantDto>
            {
                new ProductVariantDto
                {
                    Id = 0,
                    Name = "Default",
                    SKU = p.SKU,
                    Price = p.Price,
                    PriceAdjustment = 0,
                    CompareAtPrice = p.CompareAtPrice,
                    StockQuantity = p.StockQuantity
                }
            },
        Tags = p.Tags.Select(t => t.Tag).ToList(),
        MetaTitle = p.MetaTitle,
        MetaDescription = p.MetaDescription,
        CreatedAt = p.CreatedAt
    };
}
