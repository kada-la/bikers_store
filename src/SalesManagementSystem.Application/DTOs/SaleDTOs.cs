using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SalesManagementSystem.Application.DTOs;

public class SaleListDto
{
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerCity { get; set; }
    public DateTime SaleDate { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SaleItemCreateDto
{
    [Required(ErrorMessage = "Product is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, 10000, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "Unit price is required.")]
    [Range(0.01, 10000000.00, ErrorMessage = "Unit price must be greater than zero.")]
    public decimal UnitPrice { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
}

public class SaleCreateDto
{
    [Required(ErrorMessage = "Please select a customer.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid customer.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Sale date is required.")]
    public DateTime SaleDate { get; set; } = DateTime.Now;

    public List<SaleItemCreateDto> Items { get; set; } = new();

    public IReadOnlyList<CustomerLookupDto> AvailableCustomers { get; set; } = new List<CustomerLookupDto>();
    public IReadOnlyList<ProductLookupDto> AvailableProducts { get; set; } = new List<ProductLookupDto>();

    public decimal TotalAmount => Items?.Sum(i => i.Subtotal) ?? 0m;
}

public class SaleDetailsDto
{
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerCity { get; set; }
    public string? CustomerCounty { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal GrandTotal { get; set; }
    public List<SaleItemDetailDto> Items { get; set; } = new();
}

public class SaleItemDetailDto
{
    public int SaleItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
}

public record CountySalesDto(string County, int TransactionCount, decimal TotalRevenue, decimal AvgLineValue);
public record DailySalesDto(DateTime Date, int TransactionCount, int UnitsSold, decimal TotalRevenue);
public record MonthlySalesDto(int Year, int Month, string MonthName, int TransactionCount, int UnitsSold, decimal TotalRevenue);
