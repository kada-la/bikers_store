using System.Collections.Generic;
using System.Threading.Tasks;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(string supplierId);
    Task<IReadOnlyList<Supplier>> GetAllAsync();
    Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string searchTerm);
    Task AddAsync(Supplier entity);
    void Update(Supplier entity);
    void Remove(Supplier entity);
    Task<bool> ExistsAsync(string supplierId);
}
