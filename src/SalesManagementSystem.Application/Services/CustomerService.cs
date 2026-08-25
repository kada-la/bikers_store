using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;
namespace SalesManagementSystem.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyList<CustomerListDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return customers.Select(c => new CustomerListDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            City = c.City,
            County = c.County,
            Country = c.Country,
            TotalOrders = c.Sales?.Count ?? 0
        }).OrderBy(c => c.FullName).ToList();
    }

    public async Task<IReadOnlyList<CustomerListDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var customers = await _unitOfWork.Customers.SearchCustomersAsync(searchTerm.Trim());
        return customers.Select(c => new CustomerListDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            City = c.City,
            County = c.County,
            Country = c.Country,
            TotalOrders = c.Sales?.Count ?? 0
        }).OrderBy(c => c.FullName).ToList();
    }

    public async Task<CustomerDetailsDto?> GetDetailsAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) return null;

        var sales = await _unitOfWork.Sales.GetByCustomerIdAsync(id);
        var totalSpend = sales.Sum(s => s.CalculatedTotal);

        return new CustomerDetailsDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            County = customer.County,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            OrderCount = sales.Count,
            TotalSpend = totalSpend
        };
    }

    public async Task<CustomerEditDto?> GetForEditAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null) return null;

        return new CustomerEditDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            County = customer.County,
            PostalCode = customer.PostalCode,
            Country = customer.Country
        };
    }

    public async Task<IReadOnlyList<CustomerLookupDto>> GetLookupAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CustomerLookupDto(c.CustomerId, c.FullName, c.Email, c.City))
            .ToList();
    }
}
