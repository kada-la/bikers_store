using System.ComponentModel.DataAnnotations;

namespace SalesManagementSystem.Application.DTOs;

public class CategoryListDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CategoryEditDto
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}