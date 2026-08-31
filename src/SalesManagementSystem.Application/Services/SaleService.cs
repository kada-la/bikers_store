using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Services;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyList<SaleListDto>> GetAllAsync()
    {
        var sales = await _unitOfWork.Sales.GetAllWithDetailsAsync();
        return sales.Select(s => new SaleListDto
        {
            SaleId = s.SaleId,
            CustomerId = s.CustomerId,
            CustomerName = s.Customer?.FullName ?? "Unknown Customer",
            CustomerCity = s.Customer?.City,
            SaleDate = s.SaleDate,
            TotalItems = s.SaleItems.Sum(si => si.Quantity),
            TotalAmount = s.CalculatedTotal
        }).OrderByDescending(s => s.SaleDate).ToList();
    }
}
