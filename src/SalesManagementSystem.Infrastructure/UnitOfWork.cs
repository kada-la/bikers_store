using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Infrastructure.Persistence;
using SalesManagementSystem.Infrastructure.Repositories;

namespace SalesManagementSystem.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly SalesDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private ICategoryRepository? _categories;
    private ICustomerRepository? _customers;
    private IProductRepository? _products;
    private ISaleRepository? _sales;
    private IReportRepository? _reports;

    public UnitOfWork(SalesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ISaleRepository Sales => _sales ??= new SaleRepository(_context);
    public IReportRepository Reports => _reports ??= new ReportRepository(_context);

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
