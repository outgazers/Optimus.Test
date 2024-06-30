using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Users");

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new AggregateId(x));
        
        builder.Property(x => x.Password)
            .HasMaxLength(100);
        
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.Property(x => x.Role).IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Username).IsUnique();
        builder.Property(x => x.Username).IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.Permissions)
            .HasConversion(
                x => JsonConvert.SerializeObject(x,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                x => JsonConvert.DeserializeObject<IEnumerable<string>>(x,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
    }
}