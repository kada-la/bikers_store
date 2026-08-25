using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyList<ProductListDto>> GetAllAsync(bool includeInactive = true)
    {
        var products = await _unitOfWork.Products.GetAllWithCategoryAsync(includeInactive);
        return products.Select(p => new ProductListDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.CategoryName,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive
        }).OrderBy(p => p.ProductName).ToList();
    }

    public async Task<IReadOnlyList<ProductListDto>> SearchAndFilterAsync(string? searchTerm, int? categoryId, bool includeInactive = true)
    {
        var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm?.Trim(), categoryId, includeInactive);
        return products.Select(p => new ProductListDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.CategoryName,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive
        }).OrderBy(p => p.ProductName).ToList();
    }

    public async Task<ProductDetailsDto?> GetDetailsAsync(int id)
    {
        var product = await _unitOfWork.Products.GetWithCategoryByIdAsync(id);
        if (product == null) return null;

        var totalSold = product.SaleItems.Sum(si => si.Quantity);
        var totalRev = product.SaleItems.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));

        return new ProductDetailsDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.CategoryName,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            TotalSoldUnits = totalSold,
            TotalRevenue = totalRev
        };
    }

    public async Task<ProductEditDto?> GetForEditAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return null;

        var activeCategories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        var categoryDtos = activeCategories
            .OrderBy(c => c.CategoryName)
            .Select(c => new CategoryLookupDto(c.CategoryId, c.CategoryName))
            .ToList();

        return new ProductEditDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            AvailableCategories = categoryDtos
        };
    }

    public async Task<IReadOnlyList<ProductLookupDto>> GetAvailableForSaleLookupAsync()
    {
        // Only return active products with stock > 0 for creating new sales
        var products = await _unitOfWork.Products.GetAllWithCategoryAsync(includeInactive: false);
        return products
            .Where(p => p.StockQuantity > 0)
            .OrderBy(p => p.ProductName)
            .Select(p => new ProductLookupDto(p.ProductId, p.ProductName, p.Price, p.StockQuantity, p.Category.CategoryName))
            .ToList();
    }

    public async Task<ProductLookupDto?> GetProductLookupByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetWithCategoryByIdAsync(id);
        if (product == null) return null;
    
        return new ProductLookupDto(
            product.ProductId, 
            product.ProductName, 
            product.Price, 
            product.StockQuantity, 
            product.Category?.CategoryName ?? string.Empty);
    }
}
