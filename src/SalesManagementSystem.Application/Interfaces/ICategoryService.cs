using System.Collections.Generic;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryListDto>> GetAllAsync(bool includeInactive = true);
    Task<IReadOnlyList<CategoryListDto>> SearchAsync(string searchTerm, bool includeInactive = true);
    Task<CategoryEditDto?> GetForEditAsync(int id);
    Task<IReadOnlyList<CategoryLookupDto>> GetActiveLookupAsync();
    Task<Result<int>> CreateAsync(CategoryCreateDto model);
    Task<Result> UpdateAsync(CategoryEditDto model);
    Task<Result> SoftDeleteAsync(int id);
    Task<Result> ReactivateAsync(int id);
}
