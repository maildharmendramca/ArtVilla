using System.Text.Json.Serialization;

namespace IndianArtVilla.Application.DTOs;

public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    [JsonPropertyName("basePrice")]
    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }
    public int? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > 0
        ? (int)Math.Round((1 - Price / CompareAtPrice.Value) * 100) : null;
    public string? ShortDescription { get; set; }
    public string? Material { get; set; }

    [JsonPropertyName("primaryImage")]
    public string? PrimaryImageUrl { get; set; }

    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public bool IsFeatured { get; set; }
    public bool IsGiftable { get; set; }
    public bool IsNew { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    [JsonPropertyName("basePrice")]
    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }
    public int? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > 0
        ? (int)Math.Round((1 - Price / CompareAtPrice.Value) * 100) : null;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Material { get; set; }
    public string? Finish { get; set; }
    public string? Capacity { get; set; }
    public string? Dimensions { get; set; }
    public decimal WeightGrams { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public bool IsFeatured { get; set; }
    public bool IsGiftable { get; set; }
    public bool IsCustomizable { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryDisplayName { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public int CategoryIdValue { get; set; }

    public CategorySummaryDto Category { get; set; } = null!;
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("alt")]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    [JsonPropertyName("stock")]
    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public decimal WeightGrams { get; set; }
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? Finish { get; set; }
    public string? Capacity { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsGiftable { get; set; }
    public bool IsCustomizable { get; set; }
    public int CategoryId { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? Tags { get; set; }
    public List<CreateProductVariantDto>? Variants { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    public bool IsActive { get; set; } = true;
}

public class CreateProductVariantDto
{
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
}

public class ProductFilterParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int? Limit { get; set; }
    public string? CategorySlug { get; set; }
    public string? Material { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Finish { get; set; }
    public int? MinRating { get; set; }
    public bool? InStockOnly { get; set; }
    public string? SortBy { get; set; }
    public string? Search { get; set; }

    public int EffectivePageSize => Limit ?? PageSize;
}
