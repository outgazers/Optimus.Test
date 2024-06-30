using Optimus.Services.Customers.Application;

namespace Optimus.Services.Customers.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}