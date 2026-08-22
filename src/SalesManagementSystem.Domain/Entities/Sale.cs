using System;
using System.Collections.Generic;
using System.Linq;

namespace SalesManagementSystem.Domain.Entities;

public partial class Sale
{
    public int SaleId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    // Navigation property for the related Customer entity
    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public decimal CalculatedTotal => SaleItems.Sum(si => si.TotalAmount ?? (si.Quantity * si.UnitPrice));
}
