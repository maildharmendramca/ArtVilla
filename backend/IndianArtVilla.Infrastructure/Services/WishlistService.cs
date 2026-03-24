using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _uow;

    public WishlistService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<ProductListDto>> GetWishlistAsync(string userId)
    {
        return await _uow.WishlistItems.Query()
            .Include(w => w.Product).ThenInclude(p => p.Category)
            .Include(w => w.Product).ThenInclude(p => p.Images)
            .Include(w => w.Product).ThenInclude(p => p.Reviews)
            .Where(w => w.UserId == userId)
            .Select(w => new ProductListDto
            {
                Id = w.Product.Id,
                Name = w.Product.Name,
                Slug = w.Product.Slug,
                SKU = w.Product.SKU,
                Price = w.Product.Price,
                CompareAtPrice = w.Product.CompareAtPrice,
                ShortDescription = w.Product.ShortDescription,
                Material = w.Product.Material,
                PrimaryImageUrl = w.Product.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                AverageRating = w.Product.Reviews.Any() ? (decimal)w.Product.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = w.Product.Reviews.Count(),
                StockQuantity = w.Product.StockQuantity,
                IsFeatured = w.Product.IsFeatured,
                IsGiftable = w.Product.IsGiftable,
                IsNew = w.Product.CreatedAt >= DateTime.UtcNow.AddDays(-30),
                CategoryName = w.Product.Category.Name
            })
            .ToListAsync();
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var exists = await _uow.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists) return;

        _uow.WishlistItems.Add(new WishlistItem
        {
            UserId = userId,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow
        });

        await _uow.SaveChangesAsync();
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var item = await _uow.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item != null)
        {
            _uow.WishlistItems.Remove(item);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task<bool> IsInWishlistAsync(string userId, int productId)
    {
        return await _uow.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
    }
}
