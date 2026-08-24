using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Domain.Entities;
using System.Reflection;

namespace SalesManagementSystem.Infrastructure.Persistence;

public partial class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories => Set<Category>();

    public virtual DbSet<Customer> Customers => Set<Customer>();

    public virtual DbSet<Product> Products => Set<Product>();

    public virtual DbSet<Sale> Sales => Set<Sale>();

    public virtual DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public virtual DbSet<VwSalesSummary> VwSalesSummaries => Set<VwSalesSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
