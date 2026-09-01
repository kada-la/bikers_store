using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(SalesDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Products)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Category>> SearchCategoriesAsync(string searchTerm, bool includeInactive = false)
    {
        var query = _dbSet.Include(c => c.Products).AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        query = query.Where(c => c.CategoryName.Contains(searchTerm) ||
                                (c.Description != null && c.Description.Contains(searchTerm)));

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}
