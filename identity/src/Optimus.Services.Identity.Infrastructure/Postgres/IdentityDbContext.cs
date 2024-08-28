using Convey.MessageBrokers.Outbox.Messages;
using Microsoft.EntityFrameworkCore;
using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Infrastructure.Postgres;

public class IdentityDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public IdentityDbContext()
    {
    }
    
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    //     => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Identity;User id=;Password=");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identities");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}