using System.Collections.Generic;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductListDto>> GetAllAsync(bool includeInactive = true);
    Task<IReadOnlyList<ProductListDto>> SearchAndFilterAsync(string? searchTerm, int? categoryId, bool includeInactive = true);
    Task<ProductDetailsDto?> GetDetailsAsync(int id);
    Task<ProductEditDto?> GetForEditAsync(int id);
    Task<IReadOnlyList<ProductLookupDto>> GetAvailableForSaleLookupAsync();
    Task<ProductLookupDto?> GetProductLookupByIdAsync(int id);
    Task<Result<int>> CreateAsync(ProductCreateDto model);
    Task<Result> UpdateAsync(ProductEditDto model);
    Task<Result> SoftDeleteAsync(int id);
    Task<Result> ReactivateAsync(int id);
}
