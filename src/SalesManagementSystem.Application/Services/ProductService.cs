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
}
