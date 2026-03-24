using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Interfaces.Repositories;
using IndianArtVilla.Infrastructure.Data;

namespace IndianArtVilla.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Products = new Repository<Product>(db);
        Categories = new Repository<Category>(db);
        ProductImages = new Repository<ProductImage>(db);
        ProductTags = new Repository<ProductTag>(db);
        ProductVariants = new Repository<ProductVariant>(db);
        Orders = new Repository<Order>(db);
        OrderItems = new Repository<OrderItem>(db);
        OrderStatusHistories = new Repository<OrderStatusHistory>(db);
        Carts = new Repository<Cart>(db);
        CartItems = new Repository<CartItem>(db);
        Reviews = new Repository<Review>(db);
        Addresses = new Repository<Address>(db);
        WishlistItems = new Repository<WishlistItem>(db);
        Coupons = new Repository<Coupon>(db);
        CorporateInquiries = new Repository<CorporateInquiry>(db);
        RefreshTokens = new Repository<RefreshToken>(db);
    }

    public IRepository<Product> Products { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<ProductImage> ProductImages { get; }
    public IRepository<ProductTag> ProductTags { get; }
    public IRepository<ProductVariant> ProductVariants { get; }
    public IRepository<Order> Orders { get; }
    public IRepository<OrderItem> OrderItems { get; }
    public IRepository<OrderStatusHistory> OrderStatusHistories { get; }
    public IRepository<Cart> Carts { get; }
    public IRepository<CartItem> CartItems { get; }
    public IRepository<Review> Reviews { get; }
    public IRepository<Address> Addresses { get; }
    public IRepository<WishlistItem> WishlistItems { get; }
    public IRepository<Coupon> Coupons { get; }
    public IRepository<CorporateInquiry> CorporateInquiries { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _db.SaveChangesAsync(cancellationToken);

    public void Dispose() => _db.Dispose();
}
