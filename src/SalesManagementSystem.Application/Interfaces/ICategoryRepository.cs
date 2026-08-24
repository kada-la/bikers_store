using SalesManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync();
    Task<IReadOnlyList<Category>> SearchCategoriesAsync(string searchTerm, bool includeInactive = false);
    Task<bool> HasProductsAsync(int categoryId);
}
