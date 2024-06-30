namespace Optimus.Services.Identity.Infrastructure.Postgres;

public class IdentityUnitOfWork : UnitOfWork<IdentityDbContext>
{
    public IdentityUnitOfWork(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}