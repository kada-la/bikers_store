using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales", "store");

        builder.HasKey(s => s.SaleId);
        builder.Property(s => s.SaleId).HasColumnName("SaleID");

        builder.Property(s => s.CustomerId).HasColumnName("CustomerID");

        builder.Property(s => s.SaleDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasIndex(s => s.CustomerId, "idx_sales_customerid");
        builder.HasIndex(s => s.SaleDate, "idx_sales_saledate");

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Sales_Customers");

        builder.Ignore(s => s.CalculatedTotal);
    }
}
