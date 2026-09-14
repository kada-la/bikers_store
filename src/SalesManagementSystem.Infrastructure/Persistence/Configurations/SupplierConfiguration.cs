using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers", "store");

        builder.HasKey(s => s.SupplierId);
        builder.Property(s => s.SupplierId)
            .HasColumnName("SupplierId")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.SupplierName)
            .HasColumnName("SupplierName")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("Status")
            .HasMaxLength(1)
            .IsUnicode(false)
            .HasDefaultValue("N")
            .IsRequired();

        builder.Property(s => s.Email)
            .HasMaxLength(100);

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(s => s.Address)
            .HasMaxLength(255);

        builder.Property(s => s.City)
            .HasMaxLength(50);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(20);

        builder.Property(s => s.Country)
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(s => s.UpdatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(s => s.LastSyncedAtUtc);

        builder.Ignore(s => s.IsActive);
    }
}
