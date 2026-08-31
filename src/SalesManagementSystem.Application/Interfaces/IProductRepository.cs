using SalesManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(bool includeInactive = false);
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, bool includeInactive = false);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string? searchTerm, int? categoryId = null, bool includeInactive = false);
    Task<Product?> GetWithCategoryByIdAsync(int id);
    Task<bool> HasSalesAsync(int productId);
}
