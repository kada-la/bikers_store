using System.Collections.Generic;

namespace SalesManagementSystem.Application.DTOs;

public class DashboardDto
{
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }

    public IReadOnlyList<SaleListDto> RecentSales { get; set;} = new List<SaleListDto>();
    public IReadOnlyList<TopSellingProductDto> TopSellingProducts { get; set;} = new List<TopSellingProductDto>();
    public IReadOnlyList<TopCustomerDto> TopCustomers { get; set; } = new List<TopCustomerDto>();
    public IReadOnlyList<CategoryRevenueDto> CategoryRevenues { get; set; } = new List<CategoryRevenueDto>();
    public IReadOnlyList<DailySalesDto> DailySalesTrends { get; set; } = new List<DailySalesDto>();
}