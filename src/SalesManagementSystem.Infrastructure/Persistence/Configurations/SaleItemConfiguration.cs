using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items", "store");

        builder.HasKey(si => si.SaleItemId);
        builder.Property(si => si.SaleItemId).HasColumnName("SaleItemID");

        builder.Property(si => si.SaleId).HasColumnName("SaleID");
        builder.Property(si => si.ProductId).HasColumnName("ProductID");

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(10, 2)")
            .IsRequired();

        builder.Property(si => si.TotalAmount)
            .HasComputedColumnSql("([Quantity]*[UnitPrice])", stored: true)
            .HasColumnType("decimal(21, 2)")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SaleItems_Products");

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SaleItems_Sales");
    }
}
