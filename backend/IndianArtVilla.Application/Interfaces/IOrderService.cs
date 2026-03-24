using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Core.Enums;

namespace IndianArtVilla.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDetailDto> CreateOrderAsync(CheckoutRequestDto request, string userId);
    Task<PagedResult<OrderListDto>> GetUserOrdersAsync(string userId, int page, int pageSize = 10);
    Task<OrderDetailDto?> GetOrderDetailAsync(string orderNumber, string userId);
    Task<bool> CancelOrderAsync(int orderId, string userId);
    Task UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
    Task<PagedResult<OrderListDto>> GetAllOrdersAsync(int page, int pageSize, OrderStatus? status = null);
    Task<OrderDetailDto?> GetOrderDetailAdminAsync(int orderId);
}
