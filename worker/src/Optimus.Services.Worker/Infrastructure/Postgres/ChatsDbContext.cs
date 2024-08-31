using Microsoft.EntityFrameworkCore;
using Optimus.Services.Worker.Cores;

namespace Optimus.Services.Worker.Infrastructure.Postgres;

public class ChatsDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Lead> LeadExchanges { get; set; }
    
    public ChatsDbContext()
    {
    }
    
    public ChatsDbContext(DbContextOptions<ChatsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}