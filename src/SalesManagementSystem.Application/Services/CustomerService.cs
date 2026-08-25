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

    public async Task<Result<int>> CreateAsync(CustomerCreateDto model)
    {
        if (model == null)
            return Result.Failure<int>("Invalid customer data.");

        var email = model.Email.Trim().ToLower();
        var emailExists = await _unitOfWork.Customers.EmailExistsAsync(email);
        if (emailExists)
            return Result.Failure<int>($"A customer with email '{model.Email}' already exists.");

        var customer = new Customer
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = email,
            PhoneNumber = model.PhoneNumber?.Trim(),
            Address = model.Address?.Trim(),
            City = model.City?.Trim(),
            County = model.County?.Trim(),
            PostalCode = model.PostalCode?.Trim(),
            Country = model.Country?.Trim() ?? "Kenya"
        };

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.CompleteAsync();

        return Result.Success(customer.CustomerId);
    }

    public async Task<Result> UpdateAsync(CustomerEditDto model)
    {
        if (model == null)
            return Result.Failure("Invalid customer data.");

        var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
        if (customer == null)
            return Result.Failure("Customer not found.");

        var email = model.Email.Trim().ToLower();
        var emailExists = await _unitOfWork.Customers.EmailExistsAsync(email, model.CustomerId);
        if (emailExists)
            return Result.Failure($"A customer with email '{model.Email}' already exists.");

        customer.FirstName = model.FirstName.Trim();
        customer.LastName = model.LastName.Trim();
        customer.Email = email;
        customer.PhoneNumber = model.PhoneNumber?.Trim();
        customer.Address = model.Address?.Trim();
        customer.City = model.City?.Trim();
        customer.County = model.County?.Trim();
        customer.PostalCode = model.PostalCode?.Trim();
        customer.Country = model.Country?.Trim();

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null)
            return Result.Failure("Customer not found.");
    
        // Defensive check: prevent hard deletion if customer has sales records
        var hasSales = await _unitOfWork.Customers.HasSalesAsync(id);
        if (hasSales)
        {
            return Result.Failure($"Customer '{customer.FullName}' cannot be deleted because they have associated sales records.");
        }
    
        _unitOfWork.Customers.Remove(customer);
        await _unitOfWork.CompleteAsync();
    
        return Result.Success();
    }
}
