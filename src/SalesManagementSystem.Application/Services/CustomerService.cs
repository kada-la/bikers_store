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
}

