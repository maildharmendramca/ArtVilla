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
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        // Current month stats
        var thisMonthOrders = await _uow.Orders.Query()
            .Where(o => o.CreatedAt >= thisMonthStart && o.Status != OrderStatus.Cancelled)
            .ToListAsync();

        // Last month stats (for change percentage)
        var lastMonthOrders = await _uow.Orders.Query()
            .Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < thisMonthStart && o.Status != OrderStatus.Cancelled)
            .ToListAsync();

        var thisMonthRevenue = thisMonthOrders.Sum(o => o.TotalAmount);
        var lastMonthRevenue = lastMonthOrders.Sum(o => o.TotalAmount);

        var totalOrders = await _uow.Orders.CountAsync(o => o.Status != OrderStatus.Cancelled);
        var totalProducts = await _uow.Products.CountAsync(p => p.IsActive);
        var totalCustomers = _userManager.Users.Count();
        var totalRevenue = await _uow.Orders.Query()
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        // Percentage changes
        var revenueChange = lastMonthRevenue > 0
            ? Math.Round((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100, 1)
            : 0;
        var ordersChange = lastMonthOrders.Count > 0
            ? Math.Round((decimal)(thisMonthOrders.Count - lastMonthOrders.Count) / lastMonthOrders.Count * 100, 1)
            : 0;

        // Recent orders
        var recentOrders = await _uow.Orders.Query()
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
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

        // Top products
        var topProducts = await GetTopProductsInternalAsync(5);

        // Revenue by month (last 6 months)
        var revenueByMonth = await GetRevenueByMonthAsync(6);

        return new DashboardStatsDto
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            RevenueChange = revenueChange,
            OrdersChange = ordersChange,
            RecentOrders = recentOrders,
            TopProducts = topProducts,
            RevenueByMonth = revenueByMonth
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

    private async Task<List<RevenueByMonthDto>> GetRevenueByMonthAsync(int months)
    {
        var result = new List<RevenueByMonthDto>();
        var now = DateTime.UtcNow;

        for (int i = months - 1; i >= 0; i--)
        {
            var monthDate = now.AddMonths(-i);
            var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            var revenue = await _uow.Orders.Query()
                .Where(o => o.CreatedAt >= monthStart && o.CreatedAt < monthEnd && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);

            result.Add(new RevenueByMonthDto
            {
                Month = monthStart.ToString("MMM yyyy"),
                Revenue = revenue
            });
        }

        return result;
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
        var topItems = await _uow.OrderItems.Query()
            .Include(oi => oi.Order)
            .Include(oi => oi.Product).ThenInclude(p => p.Category)
            .Include(oi => oi.Product).ThenInclude(p => p.Images)
            .Include(oi => oi.Product).ThenInclude(p => p.Reviews)
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.TotalPrice)
            })
            .OrderByDescending(t => t.TotalSold)
            .Take(count)
            .ToListAsync();

        var productIds = topItems.Select(t => t.ProductId).ToList();
        var products = await _uow.Products.Query()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        return topItems.Select(t =>
        {
            var p = products.First(p => p.Id == t.ProductId);
            return new TopProductDto
            {
                Product = ProductService.MapToListDto(p),
                SoldCount = t.TotalSold,
                Revenue = t.TotalRevenue
            };
        }).ToList();
    }
}
