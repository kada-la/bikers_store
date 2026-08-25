using System.Collections.Generic;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerListDto>> GetAllAsync();
    Task<IReadOnlyList<CustomerListDto>> SearchAsync(string searchTerm);
    Task<CustomerDetailsDto?> GetDetailsAsync(int id);
    Task<CustomerEditDto?> GetForEditAsync(int id);
    Task<IReadOnlyList<CustomerLookupDto>> GetLookupAsync();
    Task<Result<int>> CreateAsync(CustomerCreateDto model);
    Task<Result> UpdateAsync(CustomerEditDto model);
    Task<Result> DeleteAsync(int id);
}
