using System.ComponentModel.DataAnnotations;

namespace SalesManagementSystem.Application.DTOs;

public class CustomerListDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? Country { get; set; }
    public int TotalOrders { get; set; }
}

public class CustomerCreateDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
    public string? PhoneNumber { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "County cannot exceed 50 characters.")]
    public string? County { get; set; }

    [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
    public string? PostalCode { get; set; }

    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
    public string? Country { get; set; } = "Kenya";
}

public class CustomerEditDto
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
    public string? PhoneNumber { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "County cannot exceed 50 characters.")]
    public string? County { get; set; }

    [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
    public string? PostalCode { get; set; }

    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
    public string? Country { get; set; }
}

public class CustomerDetailsDto
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalSpend { get; set; }
    public bool CanDelete => OrderCount == 0;
}

public record TopCustomerDto(int CustomerId, string CustomerName, string? City, string? Country, int TransactionCount, decimal TotalSpend);
public record CustomerLookupDto(int CustomerId, string FullName, string Email, string? City);
