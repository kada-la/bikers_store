using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Entities;
using SalesManagementSystem.Infrastructure.Persistence;

namespace SalesManagementSystem.Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(SalesDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Customer>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Sales)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _dbSet
            .Include(c => c.Sales)
            .Where(c => c.FirstName.Contains(searchTerm) ||
                        c.LastName.Contains(searchTerm) ||
                        c.Email.Contains(searchTerm) ||
                        (c.City != null && c.City.Contains(searchTerm)) ||
                        (c.County != null && c.County.Contains(searchTerm)))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> HasSalesAsync(int customerId)
    {
        return await _context.Sales.AnyAsync(s => s.CustomerId == customerId);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
    {
        var normalized = email.Trim().ToLower();
        return await _dbSet.AnyAsync(c =>
            c.Email.ToLower() == normalized &&
            (!excludeCustomerId.HasValue || c.CustomerId != excludeCustomerId.Value));
    }
}
