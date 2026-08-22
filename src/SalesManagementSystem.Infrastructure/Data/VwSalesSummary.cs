using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Infrastructure.Data;

public partial class VwSalesSummary
{
    public int SaleId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal? TotalSaleAmount { get; set; }
}
