using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string? userId, string? sessionId);
    Task<CartDto> AddItemAsync(AddCartItemDto dto, string? userId, string? sessionId);
    Task<CartDto> UpdateItemAsync(int cartItemId, int qty, string? userId, string? sessionId);
    Task<CartDto> RemoveItemAsync(int cartItemId, string? userId, string? sessionId);
    Task<CartDto> ApplyCouponAsync(string code, string? userId, string? sessionId);
    Task<CartDto> RemoveCouponAsync(string? userId, string? sessionId);
    Task MergeGuestCartAsync(string sessionId, string userId);
}
