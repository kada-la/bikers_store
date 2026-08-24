using SalesManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface ISaleRepository : IGenericRepository<Sale>
{
    Task<IReadOnlyList<Sale>> GetAllWithDetailsAsync();
    Task<Sale?> GetWithDetailsByIdAsync(int id);
    Task<IReadOnlyList<Sale>> GetByCustomerIdAsync(int customerId);
    Task<IReadOnlyList<Sale>> GetRecentSalesAsync(int count);
    Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
}
