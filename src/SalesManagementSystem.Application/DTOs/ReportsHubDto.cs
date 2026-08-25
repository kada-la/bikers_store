using System;
using System.Collections.Generic;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.DTOs;

public class ReportsHubDTO
{
    public IReadOnlyList<VwSalesSummary> SalesSummaries { get; set; } = new List<VwSalesSummary>();
    public IReadOnlyList<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
    public IReadOnlyList<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
    public IReadOnlyList<TopSellingProductDto> TopSellingProducts { get; set; } = new List<TopSellingProductDto>();
    public IReadOnlyList<TopCustomerDto> TopCustomers { get; set; } = new List<TopCustomerDto>();
    public IReadOnlyList<CategoryRevenueDto> CategoryRevenues { get; set; } = new List<CategoryRevenueDto>();
    public IReadOnlyList<CountySalesDto> CountySales { get; set; } = new List<CountySalesDto>();

    public decimal TotalRevenue { get; set; }
    public int TotalUnitsSold { get; set; }
}