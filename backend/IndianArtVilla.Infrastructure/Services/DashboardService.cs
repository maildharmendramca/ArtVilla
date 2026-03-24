using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Interfaces.Repositories;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var todayOrders = await _uow.Orders.Query()
            .Where(o => o.CreatedAt >= today && o.Status != OrderStatus.Cancelled)
            .ToListAsync();

        var monthlyRevenue = await _uow.Orders.Query()
            .Where(o => o.CreatedAt >= monthStart && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var pendingOrders = await _uow.Orders
            .CountAsync(o => o.Status == OrderStatus.Pending);

        var totalProducts = await _uow.Products.CountAsync(p => p.IsActive);

        var lowStockProducts = await _uow.Products
            .CountAsync(p => p.IsActive && p.TrackInventory && p.StockQuantity <= 5);

        var totalCustomers = _userManager.Users.Count();

        var revenueChart = await GetRevenueChartInternalAsync(30);
        var topProducts = await GetTopProductsInternalAsync(10);

        return new DashboardStatsDto
        {
            TodayRevenue = todayOrders.Sum(o => o.TotalAmount),
            MonthlyRevenue = monthlyRevenue,
            TotalOrdersToday = todayOrders.Count,
            PendingOrders = pendingOrders,
            TotalProducts = totalProducts,
            LowStockProducts = lowStockProducts,
            TotalCustomers = totalCustomers,
            RevenueChart = revenueChart,
            TopProducts = topProducts
        };
    }

    public async Task<IEnumerable<RevenueDataPoint>> GetRevenueChartAsync(int days = 30)
    {
        return await GetRevenueChartInternalAsync(days);
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 10)
    {
        return await GetTopProductsInternalAsync(count);
    }

    private async Task<List<RevenueDataPoint>> GetRevenueChartInternalAsync(int days)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var orders = await _uow.Orders.Query()
            .Where(o => o.CreatedAt >= startDate && o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new RevenueDataPoint
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        var result = new List<RevenueDataPoint>();
        for (var date = startDate; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var dateStr = date.ToString("yyyy-MM-dd");
            var existing = orders.FirstOrDefault(o => o.Date == dateStr);
            result.Add(existing ?? new RevenueDataPoint { Date = dateStr, Revenue = 0, OrderCount = 0 });
        }

        return result;
    }

    private async Task<List<TopProductDto>> GetTopProductsInternalAsync(int count)
    {
        return await _uow.OrderItems.Query()
            .Include(oi => oi.Order)
            .Include(oi => oi.Product).ThenInclude(p => p.Images)
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                ImageUrl = g.First().Product.Images
                    .Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()
                    ?? g.First().Product.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                TotalSold = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.TotalPrice)
            })
            .OrderByDescending(t => t.TotalSold)
            .Take(count)
            .ToListAsync();
    }
}
