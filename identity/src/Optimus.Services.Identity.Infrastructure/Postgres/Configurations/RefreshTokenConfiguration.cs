using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new AggregateId());
        builder.HasIndex(x => new { x.UserId, x.Token }).IsUnique();
        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.Ignore(x => x.Events);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
    }
}