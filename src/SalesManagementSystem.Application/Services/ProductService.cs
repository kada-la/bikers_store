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

    public async Task<Result<int>> CreateAsync(ProductCreateDto model)
    {
        if (model == null)
            return Result.Failure<int>("Invalid product data.");

        if (model.Price <= 0)
            return Result.Failure<int>("Product price must be greater than zero.");

        var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.CategoryId == model.CategoryId && c.IsActive);
        if (!categoryExists)
            return Result.Failure<int>("Selected category does not exist or is inactive.");

        var trimmedName = model.ProductName.Trim();
        var exists = await _unitOfWork.Products.ExistsAsync(p => p.ProductName.ToLower() == trimmedName.ToLower());
        if (exists)
            return Result.Failure<int>($"Product with name '{trimmedName}' already exists.");

        var product = new Product
        {
            ProductName = trimmedName,
            CategoryId = model.CategoryId,
            Description = model.Description?.Trim(),
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            IsActive = model.IsActive
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.CompleteAsync();

        return Result.Success(product.ProductId);
    }

    public async Task<Result> UpdateAsync(ProductEditDto model)
    {
        if (model == null)
            return Result.Failure("Invalid product data.");

        if (model.Price <= 0)
            return Result.Failure("Product price must be greater than zero.");

        var product = await _unitOfWork.Products.GetByIdAsync(model.ProductId);
        if (product == null)
            return Result.Failure("Product not found.");

        var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.CategoryId == model.CategoryId);
        if (!categoryExists)
            return Result.Failure("Selected category does not exist.");

        var trimmedName = model.ProductName.Trim();
        var duplicate = await _unitOfWork.Products.ExistsAsync(p => 
            p.ProductId != model.ProductId && p.ProductName.ToLower() == trimmedName.ToLower());
        if (duplicate)
            return Result.Failure($"Another product with name '{trimmedName}' already exists.");

        product.ProductName = trimmedName;
        product.CategoryId = model.CategoryId;
        product.Description = model.Description?.Trim();
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;
        product.IsActive = model.IsActive;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> SoftDeleteAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return Result.Failure("Product not found.");

        if (!product.IsActive)
            return Result.Failure("Product is already deactivated.");

        product.IsActive = false;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> ReactivateAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return Result.Failure("Product not found.");
    
        product.IsActive = true;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.CompleteAsync();
    
        return Result.Success();
    }
}
