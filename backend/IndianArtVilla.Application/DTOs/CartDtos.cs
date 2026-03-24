namespace IndianArtVilla.Application.DTOs;

public class CartDto
{
    public Guid Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal Shipping => SubTotal >= 999 ? 0 : 99;
    public decimal Tax => Math.Round(SubTotal * 0.18m, 2);
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal Total => SubTotal + Shipping + Tax - DiscountAmount;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int? VariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public bool IsGiftWrapped { get; set; }
    public string? GiftMessage { get; set; }
    public int StockQuantity { get; set; }
}

public class AddCartItemDto
{
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsGiftWrapped { get; set; }
    public string? GiftMessage { get; set; }
}

public class ApplyCouponRequest { public string Code { get; set; } = string.Empty; }
public class UpdateQuantityRequest { public int Quantity { get; set; } }
public class AddToWishlistDto { public int ProductId { get; set; } }
public class WishlistCheckDto { public bool InWishlist { get; set; } }
