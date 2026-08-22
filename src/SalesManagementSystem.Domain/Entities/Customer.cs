using System;
using System.Collections.Generic;

namespace SalesManagementSystem.Domain.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation property for the related sales
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
