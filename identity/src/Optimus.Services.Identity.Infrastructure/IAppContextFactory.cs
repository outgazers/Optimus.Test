using Optimus.Services.Identity.Application;

namespace Optimus.Services.Identity.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}