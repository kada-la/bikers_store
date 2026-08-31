using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class VwSalesSummaryConfiguration : IEntityTypeConfiguration<VwSalesSummary>
{
    public void Configure(EntityTypeBuilder<VwSalesSummary> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_sales_summary", "store");

        builder.Property(v => v.SaleId).HasColumnName("SaleID");
        builder.Property(v => v.CustomerName).HasMaxLength(101);
        builder.Property(v => v.ProductName).HasMaxLength(100);
        builder.Property(v => v.TotalSaleAmount).HasColumnType("decimal(21, 2)");
    }
}
