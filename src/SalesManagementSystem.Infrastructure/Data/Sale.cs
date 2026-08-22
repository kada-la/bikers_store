using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Infrastructure.Data;

public partial class Sale
{
    public int SaleId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SaleDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
