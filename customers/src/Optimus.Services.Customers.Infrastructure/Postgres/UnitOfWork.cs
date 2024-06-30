namespace Optimus.Services.Customers.Infrastructure.Postgres;
public class UnitOfWork : IUnitOfWork
{
    private readonly CustomersDbContext _dbContext;

    public UnitOfWork(CustomersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}