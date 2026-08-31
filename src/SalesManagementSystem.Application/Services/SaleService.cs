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

    public async Task<SaleCreateDto> PrepareCreateViewModelAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        var customerDtos = customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CustomerLookupDto(c.CustomerId, c.FullName, c.Email, c.City))
            .ToList();
    
        var products = await _unitOfWork.Products.GetAllWithCategoryAsync(includeInactive: false);
        var productDtos = products
            .Where(p => p.StockQuantity > 0)
            .OrderBy(p => p.ProductName)
            .Select(p => new ProductLookupDto(p.ProductId, p.ProductName, p.Price, p.StockQuantity, p.Category?.CategoryName ?? string.Empty))
            .ToList();
    
        return new SaleCreateDto
        {
            SaleDate = DateTime.Now,
            AvailableCustomers = customerDtos,
            AvailableProducts = productDtos,
            Items = new List<SaleItemCreateDto>
            {
                new SaleItemCreateDto { Quantity = 1 }
            }
        };
    }

    public async Task<Result<int>> CreateSaleAsync(SaleCreateDto model)
    {
        if (model == null)
            return Result.Failure<int>("Invalid sale request.");

        // 1. Validate Customer
        var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
        if (customer == null)
            return Result.Failure<int>("Selected customer does not exist.");

        // 2. Validate Line Items Count
        var validItems = model.Items?.Where(i => i.ProductId > 0 && i.Quantity > 0).ToList();
        if (validItems == null || !validItems.Any())
            return Result.Failure<int>("A sale must contain at least one valid product item.");

        // 3. Begin Atomic Unit of Work Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var sale = new Sale
            {
                CustomerId = model.CustomerId,
                SaleDate = model.SaleDate > DateTime.MinValue ? model.SaleDate : DateTime.Now
            };

            // Group items by product in case user added same product on multiple lines
            var productGroup = validItems.GroupBy(i => i.ProductId);

            foreach (var group in productGroup)
            {
                var productId = group.Key;
                var totalRequestedQuantity = group.Sum(i => i.Quantity);

                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<int>($"Product with ID {productId} does not exist.");
                }

                if (!product.IsActive)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<int>($"Product '{product.ProductName}' is inactive and cannot be sold.");
                }

                if (product.StockQuantity < totalRequestedQuantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<int>($"Insufficient stock for '{product.ProductName}'. Requested: {totalRequestedQuantity}, In Stock: {product.StockQuantity}.");
                }

                // Deduct inventory
                product.StockQuantity -= totalRequestedQuantity;
                _unitOfWork.Products.Update(product);

                // Add sale item rows (using product's current catalog price for price snapshot)
                foreach (var item in group)
                {
                    var unitPrice = item.UnitPrice > 0 ? item.UnitPrice : product.Price;

                    sale.SaleItems.Add(new SaleItem
                    {
                        ProductId = productId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice
                    });
                }
            }

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Result.Success(sale.SaleId);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure<int>($"An error occurred while saving the sale: {ex.Message}");
        }
    }

    public async Task<Result> DeleteSaleAsync(int id)
    {
        var sale = await _unitOfWork.Sales.GetWithDetailsByIdAsync(id);
        if (sale == null)
            return Result.Failure("Sale not found.");
    
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Restore inventory stock for deleted items
            foreach (var item in sale.SaleItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }
    
            _unitOfWork.Sales.Remove(sale);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();
    
            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure($"Failed to delete sale: {ex.Message}");
        }
    }
}
