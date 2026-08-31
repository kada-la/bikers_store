using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", "store");

        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.CustomerId).HasColumnName("CustomerID");

        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(c => c.Address)
            .HasMaxLength(255);

        builder.Property(c => c.City)
            .HasMaxLength(50);

        builder.Property(c => c.County)
            .HasMaxLength(50);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(10);

        builder.Property(c => c.Country)
            .HasMaxLength(50);

        builder.Ignore(c => c.FullName);
    }
}
