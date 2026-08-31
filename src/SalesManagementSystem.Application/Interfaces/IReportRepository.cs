using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface IReportRepository
{
    Task<IReadOnlyList<VwSalesSummary>> GetSalesSummaryAsync();
    Task<IReadOnlyList<DailySalesDto>> GetDailySalesAsync(int days = 30);
    Task<IReadOnlyList<MonthlySalesDto>> GetMonthlySalesAsync(int months = 12);
    Task<IReadOnlyList<TopSellingProductDto>> GetTopSellingProductsAsync(int top = 10);
    Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersBySpendAsync(int top = 10);
    Task<IReadOnlyList<CategoryRevenueDto>> GetRevenueByCategoryAsync();
    Task<IReadOnlyList<CountySalesDto>> GetSalesByCountyAsync();
    Task<(int TotalCustomers, int TotalProducts, int TotalSales, decimal TotalRevenue)> GetDashboardMetricsAsync();
}
