using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string? username = null, string email = null);
    Task<User> GetAsyncForUpdate(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User? user);
}