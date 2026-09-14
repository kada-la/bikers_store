using System;

namespace SalesManagementSystem.Domain.Entities;

public partial class Supplier
{
    public string SupplierId { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public string Status { get; set; } = "N"; // "N" = Normal/Active, "C" = Closed

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? LastSyncedAtUtc { get; set; }

    public bool IsActive => string.Equals(Status, "N", StringComparison.OrdinalIgnoreCase);
}
