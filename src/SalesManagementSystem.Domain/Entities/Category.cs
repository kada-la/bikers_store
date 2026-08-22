using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Domain.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    // Navigation property for the related products
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
