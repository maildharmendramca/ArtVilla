using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
    Task<IEnumerable<RevenueDataPoint>> GetRevenueChartAsync(int days = 30);
    Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 10);
}
