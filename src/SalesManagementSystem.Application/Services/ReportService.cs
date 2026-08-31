using System;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;

namespace SalesManagementSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var (totalCustomers, totalProducts, totalSales, totalRevenue) = await _unitOfWork.Reports.GetDashboardMetricsAsync();
        var recentSalesEntities = await _unitOfWork.Sales.GetRecentSalesAsync(5);
        var topSellingProducts = await _unitOfWork.Reports.GetTopSellingProductsAsync(5);
        var topCustomers = await _unitOfWork.Reports.GetTopCustomersBySpendAsync(5);
        var categoryRevenues = await _unitOfWork.Reports.GetRevenueByCategoryAsync();
        var dailyTrends = await _unitOfWork.Reports.GetDailySalesAsync(14);

        var recentSales = recentSalesEntities.Select(s => new SaleListDto
        {
            SaleId = s.SaleId,
            CustomerId = s.CustomerId,
            CustomerName = s.Customer?.FullName ?? "Unknown Customer",
            CustomerCity = s.Customer?.City,
            SaleDate = s.SaleDate,
            TotalItems = s.SaleItems.Sum(si => si.Quantity),
            TotalAmount = s.CalculatedTotal
        }).ToList();

        return new DashboardDto
        {
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            RecentSales = recentSales,
            TopSellingProducts = topSellingProducts,
            TopCustomers = topCustomers,
            CategoryRevenues = categoryRevenues,
            DailySalesTrends = dailyTrends
        };
    }

    public async Task<ReportsHubDto> GetReportsHubDataAsync()
    {
        var summaries = await _unitOfWork.Reports.GetSalesSummaryAsync();
        var dailySales = await _unitOfWork.Reports.GetDailySalesAsync(30);
        var monthlySales = await _unitOfWork.Reports.GetMonthlySalesAsync(12);
        var topProducts = await _unitOfWork.Reports.GetTopSellingProductsAsync(10);
        var topCustomers = await _unitOfWork.Reports.GetTopCustomersBySpendAsync(10);
        var categoryRevenues = await _unitOfWork.Reports.GetRevenueByCategoryAsync();
        var countySales = await _unitOfWork.Reports.GetSalesByCountyAsync();

        var totalRev = categoryRevenues.Sum(c => c.TotalRevenue);
        var totalUnits = categoryRevenues.Sum(c => c.UnitsSold);

        return new ReportsHubDto
        {
            SalesSummaries = summaries,
            DailySales = dailySales,
            MonthlySales = monthlySales,
            TopSellingProducts = topProducts,
            TopCustomers = topCustomers,
            CategoryRevenues = categoryRevenues,
            CountySales = countySales,
            TotalRevenue = totalRev,
            TotalUnitsSold = totalUnits
        };
    }
}
