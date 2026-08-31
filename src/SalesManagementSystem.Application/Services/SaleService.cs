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

    public async Task<SaleDetailsDto?> GetDetailsAsync(int id)
    {
        var sale = await _unitOfWork.Sales.GetWithDetailsByIdAsync(id);
        if (sale == null) return null;

        return new SaleDetailsDto
        {
            SaleId = sale.SaleId,
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer.FullName,
            CustomerEmail = sale.Customer.Email,
            CustomerPhone = sale.Customer.PhoneNumber,
            CustomerAddress = sale.Customer.Address,
            CustomerCity = sale.Customer.City,
            CustomerCounty = sale.Customer.County,
            SaleDate = sale.SaleDate,
            GrandTotal = sale.CalculatedTotal,
            Items = sale.SaleItems.Select(si => new SaleItemDetailDto
            {
                SaleItemId = si.SaleItemId,
                ProductId = si.ProductId,
                ProductName = si.Product?.ProductName ?? $"Product #{si.ProductId}",
                CategoryName = si.Product?.Category?.CategoryName ?? "N/A",
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalAmount = si.TotalAmount ?? (si.Quantity * si.UnitPrice)
            }).ToList()
        };
    }
}
