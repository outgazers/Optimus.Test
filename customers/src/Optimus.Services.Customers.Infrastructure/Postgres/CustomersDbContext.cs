using Convey.MessageBrokers.Outbox.Messages;
using Microsoft.EntityFrameworkCore;
using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Infrastructure.Postgres;

public class CustomersDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<Customer> Customers { get; set; }
    
    public CustomersDbContext()
    {
    }
    
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    //     => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Customers;User id=;Password=");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}