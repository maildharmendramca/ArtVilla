using IndianArtVilla.Core.Entities;

namespace IndianArtVilla.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<ProductImage> ProductImages { get; }
    IRepository<ProductTag> ProductTags { get; }
    IRepository<ProductVariant> ProductVariants { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<OrderStatusHistory> OrderStatusHistories { get; }
    IRepository<Cart> Carts { get; }
    IRepository<CartItem> CartItems { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Address> Addresses { get; }
    IRepository<WishlistItem> WishlistItems { get; }
    IRepository<Coupon> Coupons { get; }
    IRepository<CorporateInquiry> CorporateInquiries { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
