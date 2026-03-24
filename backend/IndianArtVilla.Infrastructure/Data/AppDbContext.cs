using IndianArtVilla.Core.Entities;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CorporateInquiry> CorporateInquiries => Set<CorporateInquiry>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ignore the domain User entity — Identity uses ApplicationUser
        builder.Ignore<User>();

        // ── Product ──────────────────────────────────────────────
        builder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasIndex(p => p.SKU).IsUnique();
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
            e.Property(p => p.CompareAtPrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.WeightGrams).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Category ─────────────────────────────────────────────
        builder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ProductImage ─────────────────────────────────────────
        builder.Entity<ProductImage>(e =>
        {
            e.HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProductTag ───────────────────────────────────────────
        builder.Entity<ProductTag>(e =>
        {
            e.HasOne(pt => pt.Product)
                .WithMany(p => p.Tags)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProductVariant ───────────────────────────────────────
        builder.Entity<ProductVariant>(e =>
        {
            e.Property(v => v.PriceAdjustment).HasColumnType("decimal(18,2)");
            e.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Order ────────────────────────────────────────────────
        builder.Entity<Order>(e =>
        {
            e.HasIndex(o => o.OrderNumber).IsUnique();
            e.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(o => o.ShippingAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── OrderItem ────────────────────────────────────────────
        builder.Entity<OrderItem>(e =>
        {
            e.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
            e.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── OrderStatusHistory ───────────────────────────────────
        builder.Entity<OrderStatusHistory>(e =>
        {
            e.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Cart ─────────────────────────────────────────────────
        builder.Entity<Cart>(e =>
        {
            e.HasIndex(c => c.UserId);
            e.HasIndex(c => c.SessionId);
        });

        // ── CartItem ─────────────────────────────────────────────
        builder.Entity<CartItem>(e =>
        {
            e.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(ci => ci.Variant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Review ───────────────────────────────────────────────
        builder.Entity<Review>(e =>
        {
            e.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Address ──────────────────────────────────────────────
        builder.Entity<Address>(e =>
        {
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── WishlistItem ─────────────────────────────────────────
        builder.Entity<WishlistItem>(e =>
        {
            e.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(w => w.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Coupon ───────────────────────────────────────────────
        builder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.Value).HasColumnType("decimal(18,2)");
            e.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
            e.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        });

        // ── CorporateInquiry ─────────────────────────────────────
        builder.Entity<CorporateInquiry>(e =>
        {
            e.HasIndex(ci => ci.Email);
        });

        // ── RefreshToken ─────────────────────────────────────────
        builder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(rt => rt.Token).IsUnique();
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
