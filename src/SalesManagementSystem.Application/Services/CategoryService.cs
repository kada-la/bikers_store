using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Services;

public class CategoryService: ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyList<CategoryListDto>> GetAllAsync(bool includeInactive = true)
    {
        var categories = includeInactive
            ? await _unitOfWork.Categories.GetAllAsync()
            : await _unitOfWork.Categories.GetActiveCategoriesAsync();

        return categories.Select(c => new CategoryListDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive,
            ProductCount = c.Products?.Count ?? 0
        }).OrderBy(c => c.CategoryName).ToList();
    }

    public async Task<IReadOnlyList<CategoryListDto>> SearchAsync(string searchTerm, bool includeInactive = true)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(includeInactive);

        var categories = await _unitOfWork.Categories.SearchCategoriesAsync(searchTerm.Trim(), includeInactive);

        return categories.Select(c => new CategoryListDto
       {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive,
            ProductCount = c.Products?.Count ?? 0
        }).OrderBy(c => c.CategoryName).ToList();
    }

    public async Task<CategoryEditDto?> GetForEditAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryEditDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public async Task<IReadOnlyList<CategoryLookupDto>> GetActiveLookupAsync()
    {
        var active = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        return active
            .OrderBy(c => c.CategoryName)
            .Select(c => new CategoryLookupDto(c.CategoryId, c.CategoryName))
            .ToList();
    }

    public async Task<Result<int>> CreateAsync(CategoryCreateDto model)
    {
        if (model == null)
            return Result.Failure<int>("Invalid category data.");
    
        var trimmedName = model.CategoryName.Trim();
        var exists = await _unitOfWork.Categories.ExistsAsync(c => c.CategoryName.ToLower() == trimmedName.ToLower());
        if (exists)
            return Result.Failure<int>($"Category '{trimmedName}' already exists.");
    
        var category = new Category
        {
            CategoryName = trimmedName,
            Description = model.Description?.Trim(),
            IsActive = model.IsActive
        };
    
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CompleteAsync();
    
        return Result.Success(category.CategoryId);
    }

    public async Task<Result> UpdateAsync(CategoryEditDto model)
    {
        if (model == null)
            return Result.Failure("Invalid category data.");

        var category = await _unitOfWork.Categories.GetByIdAsync(model.CategoryId);
        if (category == null)
            return Result.Failure("Category not found.");

        var trimmedName = model.CategoryName.Trim();
        var duplicate = await _unitOfWork.Categories.ExistsAsync(c => 
            c.CategoryId != model.CategoryId && c.CategoryName.ToLower() == trimmedName.ToLower());

        if (duplicate)
            return Result.Failure($"Another category with name '{trimmedName}' already exists.");

        category.CategoryName = trimmedName;
        category.Description = model.Description?.Trim();
        category.IsActive = model.IsActive;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> SoftDeleteAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return Result.Failure("Category not found.");

        if (!category.IsActive)
            return Result.Failure("Category is already deactivated.");

        category.IsActive = false;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> ReactivateAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return Result.Failure("Category not found.");
    
        category.IsActive = true;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.CompleteAsync();
    
        return Result.Success();
    }
}
