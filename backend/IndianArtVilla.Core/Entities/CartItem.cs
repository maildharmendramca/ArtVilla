namespace IndianArtVilla.Core.Entities;

public class CartItem
{
    public int Id { get; set; }
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int? ProductVariantId { get; set; }
    public ProductVariant? Variant { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsGiftWrapped { get; set; }
    public string? GiftMessage { get; set; }
}
