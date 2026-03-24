using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _uow;

    public CartService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<CartDto> GetCartAsync(string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);
        return MapToDto(cart);
    }

    public async Task<CartDto> AddItemAsync(AddCartItemDto dto, string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);

        var product = await _uow.Products.GetByIdAsync(dto.ProductId)
            ?? throw new NotFoundException("Product", dto.ProductId);

        if (product.StockQuantity < dto.Quantity)
            throw new BusinessRuleViolationException("Not enough stock available.");

        var existingItem = cart.Items.FirstOrDefault(i =>
            i.ProductId == dto.ProductId && i.ProductVariantId == dto.VariantId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.IsGiftWrapped = dto.IsGiftWrapped;
            existingItem.GiftMessage = dto.GiftMessage;
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                ProductVariantId = dto.VariantId,
                Quantity = dto.Quantity,
                IsGiftWrapped = dto.IsGiftWrapped,
                GiftMessage = dto.GiftMessage
            };
            cart.Items.Add(newItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();

        var reloaded = await LoadCartWithNavigationAsync(cart.Id);
        return MapToDto(reloaded!);
    }

    public async Task<CartDto> UpdateItemAsync(int cartItemId, int qty, string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException("CartItem", cartItemId);

        if (qty <= 0)
        {
            _uow.CartItems.Remove(item);
        }
        else
        {
            var product = await _uow.Products.GetByIdAsync(item.ProductId);
            if (product != null && product.StockQuantity < qty)
                throw new BusinessRuleViolationException("Not enough stock available.");

            item.Quantity = qty;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();

        var reloaded = await LoadCartWithNavigationAsync(cart.Id);
        return MapToDto(reloaded!);
    }

    public async Task<CartDto> RemoveItemAsync(int cartItemId, string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException("CartItem", cartItemId);

        _uow.CartItems.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();

        var reloaded = await LoadCartWithNavigationAsync(cart.Id);
        return MapToDto(reloaded!);
    }

    public async Task<CartDto> ApplyCouponAsync(string code, string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);
        var cartDto = MapToDto(cart);

        var coupon = await _uow.Coupons.FirstOrDefaultAsync(c => c.Code == code && c.IsActive)
            ?? throw new NotFoundException("Coupon", code);

        // Use domain method for validation
        coupon.Validate(cartDto.SubTotal);

        var discount = coupon.CalculateDiscount(cartDto.SubTotal, cartDto.Shipping);

        cartDto.CouponCode = code;
        cartDto.DiscountAmount = discount;

        return cartDto;
    }

    public async Task<CartDto> RemoveCouponAsync(string? userId, string? sessionId)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId);
        var cartDto = MapToDto(cart);
        cartDto.CouponCode = null;
        cartDto.DiscountAmount = 0;
        return cartDto;
    }

    public async Task MergeGuestCartAsync(string sessionId, string userId)
    {
        var guestCart = await _uow.Carts.Query()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId);

        if (guestCart == null || !guestCart.Items.Any())
            return;

        var userCart = await _uow.Carts.Query()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (userCart == null)
        {
            guestCart.UserId = userId;
            guestCart.SessionId = null;
            guestCart.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            foreach (var guestItem in guestCart.Items.ToList())
            {
                var existingItem = userCart.Items.FirstOrDefault(i =>
                    i.ProductId == guestItem.ProductId && i.ProductVariantId == guestItem.ProductVariantId);

                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = userCart.Id,
                        ProductId = guestItem.ProductId,
                        ProductVariantId = guestItem.ProductVariantId,
                        Quantity = guestItem.Quantity,
                        IsGiftWrapped = guestItem.IsGiftWrapped,
                        GiftMessage = guestItem.GiftMessage
                    };
                    userCart.Items.Add(newItem);
                }
            }

            userCart.UpdatedAt = DateTime.UtcNow;

            _uow.CartItems.RemoveRange(guestCart.Items);
            _uow.Carts.Remove(guestCart);
        }

        await _uow.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<Cart> GetOrCreateCartAsync(string? userId, string? sessionId)
    {
        Cart? cart = null;

        if (!string.IsNullOrEmpty(userId))
        {
            cart = await _uow.Carts.Query()
                .Include(c => c.Items).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images)
                .Include(c => c.Items).ThenInclude(ci => ci.Variant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        if (cart == null && !string.IsNullOrEmpty(sessionId))
        {
            cart = await _uow.Carts.Query()
                .Include(c => c.Items).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images)
                .Include(c => c.Items).ThenInclude(ci => ci.Variant)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = string.IsNullOrEmpty(userId) ? sessionId : null
            };
            _uow.Carts.Add(cart);
            await _uow.SaveChangesAsync();
        }

        return cart;
    }

    private async Task<Cart?> LoadCartWithNavigationAsync(Guid cartId)
    {
        return await _uow.Carts.Query()
            .Include(c => c.Items).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images)
            .Include(c => c.Items).ThenInclude(ci => ci.Variant)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    private static CartDto MapToDto(Cart cart) => new()
    {
        Id = cart.Id,
        Items = cart.Items.Select(ci => new CartItemDto
        {
            Id = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            ProductSlug = ci.Product.Slug,
            SKU = ci.Variant?.SKU ?? ci.Product.SKU,
            ImageUrl = ci.Product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                       ?? ci.Product.Images.FirstOrDefault()?.ImageUrl,
            VariantId = ci.ProductVariantId,
            VariantName = ci.Variant?.Name,
            UnitPrice = ci.Product.Price + (ci.Variant?.PriceAdjustment ?? 0),
            Quantity = ci.Quantity,
            IsGiftWrapped = ci.IsGiftWrapped,
            GiftMessage = ci.GiftMessage,
            StockQuantity = ci.Product.StockQuantity
        }).ToList()
    };
}
