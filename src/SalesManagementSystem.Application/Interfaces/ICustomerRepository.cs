using SalesManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm);
    Task<bool> HasSalesAsync(int customerId);
    Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);
}
