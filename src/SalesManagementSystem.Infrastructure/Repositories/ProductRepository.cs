using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(SalesDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(bool includeInactive = false)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, bool includeInactive = false)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId);

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string? searchTerm, int? categoryId = null, bool includeInactive = false)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p => p.ProductName.Contains(term) ||
                                    (p.Description != null && p.Description.Contains(term)));
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetWithCategoryByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.SaleItems)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<bool> HasSalesAsync(int productId)
    {
        return await _context.SaleItems.AnyAsync(si => si.ProductId == productId);
    }
}
