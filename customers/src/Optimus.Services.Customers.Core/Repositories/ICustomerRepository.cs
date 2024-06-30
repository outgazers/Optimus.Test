using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Core.Repositories;

public interface ICustomerRepository
{
    Task<bool> ExistsAsync(string fullName);
    Task<bool> ExistsAsync();
    Task<Customer> GetAsync(Guid id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}