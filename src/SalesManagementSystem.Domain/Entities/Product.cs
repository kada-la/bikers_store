using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Domain.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    // Navigation property for the related category
    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
