using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "store");

        builder.HasKey(p => p.ProductId);
        builder.Property(p => p.ProductId).HasColumnName("ProductID");

        builder.Property(p => p.CategoryId).HasColumnName("CategoryID");

        builder.Property(p => p.ProductName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(255);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(10, 2)")
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Products_Categories");

        builder.Property(p => p.SupplierId)
            .HasColumnName("SupplierId")
            .HasMaxLength(50);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Products_Suppliers");
    }
}
