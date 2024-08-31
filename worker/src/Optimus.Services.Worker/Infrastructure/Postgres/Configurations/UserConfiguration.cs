using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optimus.Services.Worker.Cores;

namespace Optimus.Services.Worker.Infrastructure.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(u => u.CrmAccountId)
            .HasColumnName("crmAccountId")
            .IsRequired();

        builder.Property(u => u.CrmToken)
            .HasColumnName("crmToken")
            .IsRequired();

        builder.Property(u => u.Credit)
            .HasColumnName("credit")
            .IsRequired();

        builder.Property(u => u.LeadExchangeCounter)
            .HasColumnName("leadExchangeCounter")
            .IsRequired();
    }
}