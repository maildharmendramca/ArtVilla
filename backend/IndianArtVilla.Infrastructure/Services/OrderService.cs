using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    public async Task<OrderDetailDto> CreateOrderAsync(CheckoutRequestDto request, string userId)
    {
        var cart = await _uow.Carts.Query()
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Variant)
            .FirstOrDefaultAsync(c => c.UserId == userId)
            ?? throw new BusinessRuleViolationException("Cart is empty.");

        if (!cart.Items.Any())
            throw new BusinessRuleViolationException("Cart is empty.");

        var address = await _uow.Addresses.FirstOrDefaultAsync(a => a.Id == request.ShippingAddressId)
            ?? throw new NotFoundException("Address", request.ShippingAddressId);

        if (address.UserId != userId)
            throw new UnauthorizedDomainException("Address does not belong to user.");

        var orderNumber = await GenerateOrderNumberAsync();

        decimal subTotal = 0;
        var orderItems = new List<OrderItem>();

        foreach (var ci in cart.Items)
        {
            var unitPrice = ci.Product.Price + (ci.Variant?.PriceAdjustment ?? 0);
            var totalPrice = unitPrice * ci.Quantity;
            subTotal += totalPrice;

            var orderItem = new OrderItem
            {
                ProductId = ci.ProductId,
                ProductVariantId = ci.ProductVariantId,
                ProductName = ci.Product.Name,
                VariantName = ci.Variant?.Name,
                ProductSKU = ci.Variant?.SKU ?? ci.Product.SKU,
                UnitPrice = unitPrice,
                Quantity = ci.Quantity,
                TotalPrice = totalPrice,
                IsGiftWrapped = ci.IsGiftWrapped,
                GiftMessage = ci.GiftMessage
            };

            orderItems.Add(orderItem);

            // Use domain method for stock deduction
            ci.Product.DeductStock(ci.Quantity);
        }

        decimal shippingAmount = subTotal >= 999 ? 0 : 99;
        decimal taxAmount = Math.Round(subTotal * 0.18m, 2);
        decimal discountAmount = 0;

        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            var coupon = await _uow.Coupons.FirstOrDefaultAsync(c =>
                c.Code == request.CouponCode && c.IsActive);

            if (coupon != null && (!coupon.ExpiresAt.HasValue || coupon.ExpiresAt > DateTime.UtcNow))
            {
                if (!coupon.MinOrderAmount.HasValue || subTotal >= coupon.MinOrderAmount)
                {
                    discountAmount = coupon.CalculateDiscount(subTotal, shippingAmount);

                    if (coupon.Type == CouponType.FreeShipping)
                        shippingAmount = 0;

                    coupon.IncrementUsage();
                }
            }
        }

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            ShippingAmount = shippingAmount,
            DiscountAmount = discountAmount,
            TaxAmount = taxAmount,
            TotalAmount = subTotal + shippingAmount + taxAmount - discountAmount,
            CouponCode = request.CouponCode,
            Notes = request.Notes,
            IsGiftOrder = request.IsGiftOrder,
            GiftMessage = request.GiftMessage,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            ShippingAddressId = request.ShippingAddressId,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems,
            StatusHistory = new List<OrderStatusHistory>
            {
                new()
                {
                    Status = OrderStatus.Pending,
                    Note = "Order placed",
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        _uow.Orders.Add(order);

        // Clear cart
        _uow.CartItems.RemoveRange(cart.Items);
        _uow.Carts.Remove(cart);

        await _uow.SaveChangesAsync();

        var savedOrder = await LoadOrderWithNavigationAsync(order.Id);
        return MapToDetailDto(savedOrder!);
    }

    public async Task<PagedResult<OrderListDto>> GetUserOrdersAsync(string userId, int page, int pageSize = 10)
    {
        var query = _uow.Orders.Query()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Count,
                PaymentStatus = o.PaymentStatus,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<OrderListDto>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    public async Task<OrderDetailDto?> GetOrderDetailAsync(string orderNumber, string userId)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.Items).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
            .Include(o => o.ShippingAddress)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.UserId == userId);

        if (order == null) return null;

        var dto = MapToDetailDto(order);
        dto.Customer = await GetCustomerDto(order.UserId);
        return dto;
    }

    public async Task<bool> CancelOrderAsync(int orderId, string userId)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.Items).ThenInclude(oi => oi.Product)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            return false;

        // Use domain method for cancellation (includes validation)
        order.Cancel();

        // Restore stock using domain method
        foreach (var item in order.Items)
            item.Product.RestoreStock(item.Quantity);

        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _uow.Orders.Query()
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException("Order", orderId);

        // Use domain method for status update
        order.UpdateStatus(dto.Status, dto.TrackingNumber, dto.TrackingUrl, dto.Note);

        await _uow.SaveChangesAsync();
    }

    public async Task<PagedResult<OrderListDto>> GetAllOrdersAsync(int page, int pageSize, OrderStatus? status = null)
    {
        var query = _uow.Orders.Query()
            .Include(o => o.Items)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        var ordered = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await ordered.CountAsync();
        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Count,
                PaymentStatus = o.PaymentStatus,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<OrderListDto>
        {
            Items = items,
            Pagination = new PaginationMeta
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }

    public async Task<OrderDetailDto?> GetOrderDetailAdminAsync(int orderId)
    {
        var order = await LoadOrderWithNavigationAsync(orderId);
        if (order == null) return null;

        var dto = MapToDetailDto(order);
        dto.Customer = await GetCustomerDto(order.UserId);
        return dto;
    }

    // Fix race condition: use MAX order number instead of COUNT
    private async Task<string> GenerateOrderNumberAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"IAV-{today}-";

        var lastOrderNumber = await _uow.Orders.Query()
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .Select(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int nextSequence = 1;
        if (lastOrderNumber != null)
        {
            var sequencePart = lastOrderNumber.Substring(prefix.Length);
            if (int.TryParse(sequencePart, out var lastSequence))
                nextSequence = lastSequence + 1;
        }

        return $"{prefix}{nextSequence:D4}";
    }

    private async Task<Order?> LoadOrderWithNavigationAsync(int orderId)
    {
        return await _uow.Orders.Query()
            .Include(o => o.Items).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
            .Include(o => o.ShippingAddress)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    private async Task<UserDto?> GetCustomerDto(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.Phone
        };
    }

    private static OrderDetailDto MapToDetailDto(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        Status = o.Status,
        SubTotal = o.SubTotal,
        ShippingAmount = o.ShippingAmount,
        DiscountAmount = o.DiscountAmount,
        TaxAmount = o.TaxAmount,
        TotalAmount = o.TotalAmount,
        CouponCode = o.CouponCode,
        Notes = o.Notes,
        IsGiftOrder = o.IsGiftOrder,
        GiftMessage = o.GiftMessage,
        PaymentStatus = o.PaymentStatus,
        PaymentMethod = o.PaymentMethod,
        TrackingNumber = o.TrackingNumber,
        TrackingUrl = o.TrackingUrl,
        CreatedAt = o.CreatedAt,
        ShippedAt = o.ShippedAt,
        DeliveredAt = o.DeliveredAt,
        ShippingAddress = new AddressDto
        {
            Id = o.ShippingAddress.Id,
            FullName = o.ShippingAddress.FullName,
            Phone = o.ShippingAddress.Phone,
            AddressLine1 = o.ShippingAddress.AddressLine1,
            AddressLine2 = o.ShippingAddress.AddressLine2,
            City = o.ShippingAddress.City,
            State = o.ShippingAddress.State,
            PinCode = o.ShippingAddress.PinCode,
            Country = o.ShippingAddress.Country,
            IsDefault = o.ShippingAddress.IsDefault,
            Type = o.ShippingAddress.Type
        },
        Items = o.Items.Select(oi => new OrderItemDto
        {
            Id = oi.Id,
            ProductId = oi.ProductId,
            ProductName = oi.ProductName,
            VariantName = oi.VariantName,
            ProductSKU = oi.ProductSKU,
            UnitPrice = oi.UnitPrice,
            Quantity = oi.Quantity,
            TotalPrice = oi.TotalPrice,
            IsGiftWrapped = oi.IsGiftWrapped,
            GiftMessage = oi.GiftMessage,
            ImageUrl = oi.Product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                       ?? oi.Product.Images.FirstOrDefault()?.ImageUrl
        }).ToList(),
        StatusHistory = o.StatusHistory.OrderByDescending(h => h.CreatedAt).Select(h => new OrderStatusHistoryDto
        {
            Status = h.Status,
            Note = h.Note,
            CreatedAt = h.CreatedAt
        }).ToList()
    };
}
