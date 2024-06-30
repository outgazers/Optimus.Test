using Convey.MessageBrokers.Outbox.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Optimus.Services.Customers.Infrastructure.Postgres.Configurations;

public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder
            .Property(x => x.Headers)
            .HasConversion(
                x => JsonConvert.SerializeObject(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                x => JsonConvert.DeserializeObject<Dictionary<string, object>>(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
            )
            .HasColumnType("json");
        
        builder
            .Property(x => x.Message)
            .HasConversion(
                x => JsonConvert.SerializeObject(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                x => JsonConvert.DeserializeObject<object>(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
            )
            .HasColumnType("json");
        
        builder
            .Property(x => x.MessageContext)
            .HasConversion(
                x => JsonConvert.SerializeObject(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                x => JsonConvert.DeserializeObject<object>(x,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
            )
            .HasColumnType("json");
    }
}