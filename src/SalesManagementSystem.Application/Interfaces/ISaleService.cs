using System.Collections.Generic;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface ISaleService
{
    Task<IReadOnlyList<SaleListDto>> GetAllAsync();
    Task<SaleDetailsDto?> GetDetailsAsync(int id);
    Task<SaleCreateDto> PrepareCreateViewModelAsync();
    Task<Result<int>> CreateSaleAsync(SaleCreateDto model);
    Task<Result> DeleteSaleAsync(int id);
}
