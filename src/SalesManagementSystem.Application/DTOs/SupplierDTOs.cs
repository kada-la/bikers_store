using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SalesManagementSystem.Application.DTOs;

public class SupplierListDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Status { get; set; } = "N";
    public bool IsActive => string.Equals(Status, "N", StringComparison.OrdinalIgnoreCase);
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
}

public class SupplierDetailsDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Status { get; set; } = "N";
    public bool IsActive => string.Equals(Status, "N", StringComparison.OrdinalIgnoreCase);
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
    public bool IsLiveFromErp { get; set; }
}

public class CreateSupplierDto
{
    [Required(ErrorMessage = "Supplier ID is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Supplier ID must be between 2 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Supplier ID can only contain alphanumeric characters, underscores, and hyphens.")]
    public string SupplierId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Supplier name must be between 2 and 200 characters.")]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Alias name cannot exceed 100 characters.")]
    public string? AliasName { get; set; }

    [StringLength(100, ErrorMessage = "External reference cannot exceed 100 characters.")]
    public string? ExternalReference { get; set; }

    [StringLength(50, ErrorMessage = "Supplier group ID cannot exceed 50 characters.")]
    public string? SupplierGroupId { get; set; } = "10";

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 digits.")]
    public string? PhoneNumber { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string? City { get; set; }

    [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters.")]
    public string? PostalCode { get; set; }

    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
    public string? Country { get; set; } = "Kenya";

    [StringLength(10, ErrorMessage = "Currency cannot exceed 10 characters.")]
    public string? Currency { get; set; } = "KES";

    [StringLength(10, ErrorMessage = "Payment terms cannot exceed 10 characters.")]
    public string? PaymentTerms { get; set; } = "30";
}

public class UpdateSupplierDto
{
    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Supplier name must be between 2 and 200 characters.")]
    public string SupplierName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 digits.")]
    public string? PhoneNumber { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string? City { get; set; }

    [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters.")]
    public string? PostalCode { get; set; }

    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
    public string? Country { get; set; }

    public string? Status { get; set; }
}

/// <summary>
/// DTO representing the Unit4 ERP Agresso Supplier payload.
/// Matches Unit4 SupplierDto schema properties.
/// </summary>
public class Unit4SupplierDto
{
    [JsonPropertyName("supplierId")]
    public string? SupplierId { get; set; }

    [JsonPropertyName("supplierName")]
    public string? SupplierName { get; set; }

    [JsonPropertyName("name")]
    public string? Name
    {
        get => SupplierName;
        set => SupplierName ??= value;
    }

    [JsonPropertyName("aliasName")]
    public string? AliasName { get; set; }

    [JsonPropertyName("externalReference")]
    public string? ExternalReference { get; set; }

    [JsonPropertyName("supplierGroupId")]
    public string? SupplierGroupId { get; set; }

    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("paymentTerms")]
    public string? PaymentTerms { get; set; }
}

public record SupplierLookupDto(string SupplierId, string SupplierName, string? City, bool IsActive);
