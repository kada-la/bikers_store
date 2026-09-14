using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierListDto>> GetAllAsync(bool liveErp = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierListDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<SupplierDetailsDto?> GetByIdAsync(string supplierId, bool fromErp = false, CancellationToken cancellationToken = default);
    Task<Result<SupplierDetailsDto>> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string supplierId, UpdateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<Result> CloseAsync(string supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
}
