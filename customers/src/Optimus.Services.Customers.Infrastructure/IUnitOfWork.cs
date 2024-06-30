namespace Optimus.Services.Customers.Infrastructure;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}