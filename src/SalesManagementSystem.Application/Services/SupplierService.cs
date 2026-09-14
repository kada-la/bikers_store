using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnit4SupplierClient _unit4Client;

    public SupplierService(
        IUnitOfWork unitOfWork,
        IUnit4SupplierClient unit4Client)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _unit4Client = unit4Client ?? throw new ArgumentNullException(nameof(unit4Client));
    }

    public async Task<IReadOnlyList<SupplierListDto>> GetAllAsync(bool liveErp = false, CancellationToken cancellationToken = default)
    {
        if (liveErp)
        {
            var erpResult = await _unit4Client.SearchSuppliersAsync(cancellationToken: cancellationToken);
            if (erpResult.IsSuccess && erpResult.Value != null)
            {
                return erpResult.Value.Select(s => new SupplierListDto
                {
                    SupplierId = s.SupplierId ?? string.Empty,
                    SupplierName = s.Name ?? string.Empty,
                    Status = s.Status ?? "N",
                    Email = s.Email,
                    PhoneNumber = s.Telephone,
                    City = s.City,
                    Country = s.CountryCode,
                    LastSyncedAtUtc = DateTime.UtcNow
                }).OrderBy(s => s.SupplierName).ToList();
            }
        }

        var localSuppliers = await _unitOfWork.Suppliers.GetAllAsync();
        return localSuppliers.Select(s => new SupplierListDto
        {
            SupplierId = s.SupplierId,
            SupplierName = s.SupplierName,
            Status = s.Status,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            City = s.City,
            Country = s.Country,
            LastSyncedAtUtc = s.LastSyncedAtUtc
        }).OrderBy(s => s.SupplierName).ToList();
    }

    public async Task<IReadOnlyList<SupplierListDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(false, cancellationToken);

        var suppliers = await _unitOfWork.Suppliers.SearchSuppliersAsync(searchTerm.Trim());
        return suppliers.Select(s => new SupplierListDto
        {
            SupplierId = s.SupplierId,
            SupplierName = s.SupplierName,
            Status = s.Status,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            City = s.City,
            Country = s.Country,
            LastSyncedAtUtc = s.LastSyncedAtUtc
        }).OrderBy(s => s.SupplierName).ToList();
    }

    public async Task<SupplierDetailsDto?> GetByIdAsync(string supplierId, bool fromErp = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(supplierId)) return null;

        if (fromErp)
        {
            var erpResult = await _unit4Client.GetSupplierAsync(supplierId, null, cancellationToken);
            if (erpResult.IsSuccess && erpResult.Value != null)
            {
                var s = erpResult.Value;
                return new SupplierDetailsDto
                {
                    SupplierId = s.SupplierId ?? supplierId,
                    SupplierName = s.Name ?? string.Empty,
                    Status = s.Status ?? "N",
                    Email = s.Email,
                    PhoneNumber = s.Telephone,
                    Address = s.Address,
                    City = s.City,
                    PostalCode = s.PostalCode,
                    Country = s.CountryCode,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                    LastSyncedAtUtc = DateTime.UtcNow,
                    IsLiveFromErp = true
                };
            }
        }

        var local = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
        if (local == null) return null;

        return new SupplierDetailsDto
        {
            SupplierId = local.SupplierId,
            SupplierName = local.SupplierName,
            Status = local.Status,
            Email = local.Email,
            PhoneNumber = local.PhoneNumber,
            Address = local.Address,
            City = local.City,
            PostalCode = local.PostalCode,
            Country = local.Country,
            CreatedAtUtc = local.CreatedAtUtc,
            UpdatedAtUtc = local.UpdatedAtUtc,
            LastSyncedAtUtc = local.LastSyncedAtUtc,
            IsLiveFromErp = false
        };
    }

    public async Task<Result<SupplierDetailsDto>> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return Result.Failure<SupplierDetailsDto>("Invalid supplier data.");

        var id = dto.SupplierId.Trim();
        var exists = await _unitOfWork.Suppliers.ExistsAsync(id);
        if (exists)
            return Result.Failure<SupplierDetailsDto>($"A supplier with ID '{id}' already exists locally.");

        // 1. Build Unit4 DTO payload
        var unit4Payload = new Unit4SupplierDto
        {
            SupplierId = id,
            SupplierName = dto.SupplierName.Trim(),
            AliasName = !string.IsNullOrWhiteSpace(dto.AliasName) ? dto.AliasName.Trim() : dto.SupplierName.Trim(),
            ExternalReference = !string.IsNullOrWhiteSpace(dto.ExternalReference) ? dto.ExternalReference.Trim() : id,
            SupplierGroupId = !string.IsNullOrWhiteSpace(dto.SupplierGroupId) ? dto.SupplierGroupId.Trim() : "10",
            Status = "N",
            Email = dto.Email?.Trim(),
            Telephone = dto.PhoneNumber?.Trim(),
            Address = dto.Address?.Trim(),
            City = dto.City?.Trim(),
            PostalCode = dto.PostalCode?.Trim(),
            CountryCode = "KE",
            Currency = dto.Currency?.Trim() ?? "KES",
            PaymentTerms = dto.PaymentTerms?.Trim() ?? "30"
        };

        // 2. Post to Unit4 ERP
        DateTime? lastSynced = null;
        var erpResult = await _unit4Client.CreateSupplierAsync(unit4Payload, cancellationToken);
        if (erpResult.IsSuccess)
        {
            lastSynced = DateTime.UtcNow;
        }

        // 3. Persist local shadow record in Bikers Store database
        var localSupplier = new Supplier
        {
            SupplierId = id,
            SupplierName = dto.SupplierName.Trim(),
            Status = "N",
            Email = dto.Email?.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            Address = dto.Address?.Trim(),
            City = dto.City?.Trim(),
            PostalCode = dto.PostalCode?.Trim(),
            Country = dto.Country?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            LastSyncedAtUtc = lastSynced
        };

        await _unitOfWork.Suppliers.AddAsync(localSupplier);
        await _unitOfWork.CompleteAsync(cancellationToken);

        var details = new SupplierDetailsDto
        {
            SupplierId = localSupplier.SupplierId,
            SupplierName = localSupplier.SupplierName,
            Status = localSupplier.Status,
            Email = localSupplier.Email,
            PhoneNumber = localSupplier.PhoneNumber,
            Address = localSupplier.Address,
            City = localSupplier.City,
            PostalCode = localSupplier.PostalCode,
            Country = localSupplier.Country,
            CreatedAtUtc = localSupplier.CreatedAtUtc,
            UpdatedAtUtc = localSupplier.UpdatedAtUtc,
            LastSyncedAtUtc = localSupplier.LastSyncedAtUtc,
            IsLiveFromErp = erpResult.IsSuccess
        };

        return Result.Success(details);
    }

    public async Task<Result> UpdateAsync(string supplierId, UpdateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null) return Result.Failure("Invalid supplier data.");

        var local = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
        if (local == null) return Result.Failure($"Supplier '{supplierId}' not found.");

        local.SupplierName = dto.SupplierName.Trim();
        local.Email = dto.Email?.Trim();
        local.PhoneNumber = dto.PhoneNumber?.Trim();
        local.Address = dto.Address?.Trim();
        local.City = dto.City?.Trim();
        local.PostalCode = dto.PostalCode?.Trim();
        local.Country = dto.Country?.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Status))
            local.Status = dto.Status.Trim().ToUpperInvariant();
        local.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Suppliers.Update(local);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CloseAsync(string supplierId, CancellationToken cancellationToken = default)
    {
        var local = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
        if (local == null) return Result.Failure($"Supplier '{supplierId}' not found.");

        // 1. Notify Unit4 ERP to close the supplier (sets status to 'C')
        var erpResult = await _unit4Client.CloseSupplierAsync(supplierId, null, cancellationToken);

        // 2. Set local status to 'C' (Closed)
        local.Status = "C";
        local.UpdatedAtUtc = DateTime.UtcNow;
        if (erpResult.IsSuccess)
            local.LastSyncedAtUtc = DateTime.UtcNow;

        _unitOfWork.Suppliers.Update(local);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyList<SupplierLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
        return suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.SupplierName)
            .Select(s => new SupplierLookupDto(s.SupplierId, s.SupplierName, s.City, s.IsActive))
            .ToList();
    }
}
