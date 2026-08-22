using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Domain.Entities;

public partial class SaleItem
{
    public int SaleItemId { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Database Persisted Computed Column (Quantity * UnitPrice)
    public decimal? TotalAmount { get; set; }

    // Navigation properties for the related Product and Sale entities
    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}
