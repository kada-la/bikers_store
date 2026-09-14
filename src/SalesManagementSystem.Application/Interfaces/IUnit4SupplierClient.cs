using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Application.Interfaces;

public interface IUnit4SupplierClient
{
    Task<Result<Unit4SupplierDto>> CreateSupplierAsync(Unit4SupplierDto dto, CancellationToken cancellationToken = default);
    Task<Result<Unit4SupplierDto>> GetSupplierAsync(string supplierId, string? companyId = null, CancellationToken cancellationToken = default);
    Task<Result> CloseSupplierAsync(string supplierId, string? companyId = null, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Unit4SupplierDto>>> SearchSuppliersAsync(string? filter = null, int offset = 0, int limit = 50, CancellationToken cancellationToken = default);
}
