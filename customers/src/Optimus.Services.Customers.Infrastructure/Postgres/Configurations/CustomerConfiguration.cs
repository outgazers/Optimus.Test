using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.ValueObjects;

namespace Optimus.Services.Customers.Infrastructure.Postgres.Configurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Customers");
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new AggregateId(x));
        builder.Property(x => x.FullName)
            .HasConversion(x => x.Value, x => new FullName(x));
        builder.Property(x => x.Address)
            .HasConversion(x => x.Value, x => new Address(x));

        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName).HasMaxLength(50);

        builder.Property(x => x.Address).HasMaxLength(500);
        
        builder.Ignore(x => x.Events);
    }
}