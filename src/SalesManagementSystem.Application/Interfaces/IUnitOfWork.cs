using System;
using System.Threading;
using System.Threading.Tasks;

namespace SalesManagementSystem.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
	ICategoryRepository Categories { get; }
	ICustomerRepository Customers { get; }
	IProductRepository Products { get; }
	ISaleRepository Sales { get; }
	IReportRepository Reports { get; }

	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
	Task BeginTransactionAsync();
	Task CommitTransactionAsync();
	Task RollbackTransactionAsync();
}
