using SalesManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public record CategoryRevenueDto(string CategoryName, int LineItemsSold, int UnitsSold, decimal TotalRevenue);
public record TopCustomerDto(int CustomerId, string CustomerName, string? City, string? Country, int TransactionCount, decimal TotalSpend);
public record CountySalesDto(string County, int TransactionCount, decimal TotalRevenue, decimal AvgLineValue);
public record DailySalesDto(DateTime Date, int TransactionCount, int UnitsSold, decimal TotalRevenue);
public record MonthlySalesDto(int Year, int Month, string MonthName, int TransactionCount, int UnitsSold, decimal TotalRevenue);
public record TopSellingProductDto(int ProductId, string ProductName, string CategoryName, int UnitsSold, decimal TotalRevenue);

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
