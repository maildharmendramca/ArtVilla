using IndianArtVilla.Core.Exceptions;

namespace IndianArtVilla.Core.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
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
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public static string GenerateSlug(string name) =>
        name.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");

    public void DeductStock(int quantity)
    {
        if (!TrackInventory) return;

        if (StockQuantity < quantity)
            throw new BusinessRuleViolationException($"Insufficient stock for {Name}. Available: {StockQuantity}, Requested: {quantity}.");

        StockQuantity -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        if (TrackInventory)
            StockQuantity += quantity;
    }
}
