using Microsoft.EntityFrameworkCore;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Repositories;

namespace Optimus.Services.Customers.Infrastructure.Postgres.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext _context;
    private readonly DbSet<Customer> _customers;

    public CustomerRepository(CustomersDbContext context)
    {
        _context = context;
        _customers = _context.Customers;
    }

    public Task<bool> ExistsAsync(string fullName)
        => _customers.AnyAsync(x => x.FullName == fullName);
    
    public Task<bool> ExistsAsync()
        => _customers.AnyAsync();

    public  async Task<Customer> GetAsync(Guid id)
        => await _customers.SingleOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Customer customer)
    {
        await _customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}