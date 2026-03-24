using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<ProductListDto>> GetWishlistAsync(string userId);
    Task AddToWishlistAsync(string userId, int productId);
    Task RemoveFromWishlistAsync(string userId, int productId);
    Task<bool> IsInWishlistAsync(string userId, int productId);
}
