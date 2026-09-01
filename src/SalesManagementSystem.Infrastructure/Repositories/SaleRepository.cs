using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class SaleRepository : GenericRepository<Sale>, ISaleRepository
{
    public SaleRepository(SalesDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Sale>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Sale?> GetWithDetailsByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(s => s.SaleId == id);
    }

    public async Task<IReadOnlyList<Sale>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Where(s => s.CustomerId == customerId)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetRecentSalesAsync(int count)
    {
        return await _dbSet
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .AsNoTracking()
            .ToListAsync();
    }
}
