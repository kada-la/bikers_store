using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly SalesDbContext _context;

    public ReportRepository(SalesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<VwSalesSummary>> GetSalesSummaryAsync()
    {
        return await _context.VwSalesSummaries.AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<DailySalesDto>> GetDailySalesAsync(int days = 30)
    {
        var cutoff = DateTime.Today.AddDays(-days);

        var sales = await _context.Sales
            .Where(s => s.SaleDate >= cutoff)
            .Include(s => s.SaleItems)
            .AsNoTracking()
            .ToListAsync();

        var grouped = sales
            .GroupBy(s => s.SaleDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySalesDto(
                g.Key,
                g.Count(),
                g.SelectMany(s => s.SaleItems).Sum(si => si.Quantity),
                g.SelectMany(s => s.SaleItems).Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice))
            ))
            .ToList();

        return grouped;
    }

    public async Task<IReadOnlyList<MonthlySalesDto>> GetMonthlySalesAsync(int months = 12)
    {
        var cutoff = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-months + 1);

        var sales = await _context.Sales
            .Where(s => s.SaleDate >= cutoff)
            .Include(s => s.SaleItems)
            .AsNoTracking()
            .ToListAsync();

        var grouped = sales
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month);
                var txCount = g.Count();
                var units = g.SelectMany(s => s.SaleItems).Sum(si => si.Quantity);
                var rev = g.SelectMany(s => s.SaleItems).Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));

                return new MonthlySalesDto(g.Key.Year, g.Key.Month, $"{monthName} {g.Key.Year}", txCount, units, rev);
            })
            .ToList();

        return grouped;
    }

    public async Task<IReadOnlyList<TopSellingProductDto>> GetTopSellingProductsAsync(int top = 10)
    {
        var items = await _context.SaleItems
            .Include(si => si.Product)
                .ThenInclude(p => p.Category)
            .AsNoTracking()
            .ToListAsync();

        var grouped = items
            .GroupBy(si => new { si.ProductId, ProductName = si.Product?.ProductName ?? $"Product #{si.ProductId}", CategoryName = si.Product?.Category?.CategoryName ?? "General" })
            .Select(g => new TopSellingProductDto(
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.CategoryName,
                g.Sum(si => si.Quantity),
                g.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice))
            ))
            .OrderByDescending(x => x.UnitsSold)
            .Take(top)
            .ToList();

        return grouped;
    }

    public async Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersBySpendAsync(int top = 10)
    {
        var customers = await _context.Customers
            .Include(c => c.Sales)
                .ThenInclude(s => s.SaleItems)
            .AsNoTracking()
            .ToListAsync();

        var ranked = customers
            .Select(c =>
            {
                var txCount = c.Sales.Count;
                var totalSpend = c.Sales.SelectMany(s => s.SaleItems).Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));
                return new TopCustomerDto(c.CustomerId, c.FullName, c.City, c.Country, txCount, totalSpend);
            })
            .Where(c => c.TotalSpend > 0)
            .OrderByDescending(c => c.TotalSpend)
            .Take(top)
            .ToList();

        return ranked;
    }

    public async Task<IReadOnlyList<CategoryRevenueDto>> GetRevenueByCategoryAsync()
    {
        var items = await _context.SaleItems
            .Include(si => si.Product)
                .ThenInclude(p => p.Category)
            .AsNoTracking()
            .ToListAsync();

        var grouped = items
            .GroupBy(si => si.Product?.Category?.CategoryName ?? "Uncategorized")
            .Select(g => new CategoryRevenueDto(
                g.Key,
                g.Count(),
                g.Sum(si => si.Quantity),
                g.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice))
            ))
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        return grouped;
    }

    public async Task<IReadOnlyList<CountySalesDto>> GetSalesByCountyAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .AsNoTracking()
            .ToListAsync();

        var grouped = sales
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Customer?.County) ? "Not Specified" : s.Customer.County)
            .Select(g =>
            {
                var txCount = g.Count();
                var items = g.SelectMany(s => s.SaleItems).ToList();
                var totalRev = items.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));
                var avgLine = items.Any() ? totalRev / items.Count : 0m;

                return new CountySalesDto(g.Key, txCount, totalRev, decimal.Round(avgLine, 2));
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        return grouped;
    }

    public async Task<(int TotalCustomers, int TotalProducts, int TotalSales, decimal TotalRevenue)> GetDashboardMetricsAsync()
    {
        var totalCustomers = await _context.Customers.CountAsync();
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
        var totalSales = await _context.Sales.CountAsync();

        var saleItems = await _context.SaleItems.AsNoTracking().ToListAsync();
        var totalRevenue = saleItems.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));

        return (totalCustomers, totalProducts, totalSales, totalRevenue);
    }
}
