using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly SalesDbContext _context;
    private readonly DbSet<Supplier> _dbSet;

    public SupplierRepository(SalesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Supplier>();
    }

    public async Task<Supplier?> GetByIdAsync(string supplierId)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.SupplierId == supplierId);
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string searchTerm)
    {
        return await _dbSet
            .Where(s => s.SupplierName.Contains(searchTerm) ||
                        s.SupplierId.Contains(searchTerm) ||
                        (s.Email != null && s.Email.Contains(searchTerm)) ||
                        (s.City != null && s.City.Contains(searchTerm)) ||
                        (s.Country != null && s.Country.Contains(searchTerm)))
            .AsNoTracking()
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    public async Task AddAsync(Supplier entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(Supplier entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(Supplier entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(string supplierId)
    {
        return await _dbSet.AnyAsync(s => s.SupplierId == supplierId);
    }
}
