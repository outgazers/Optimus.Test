using System;
using System.Threading.Tasks;

namespace Optimus.Services.Identity.Infrastructure
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<Task> action);
    }
}