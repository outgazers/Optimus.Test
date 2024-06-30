using System.Linq.Expressions;
using Optimus.Services.Customers.Application.Storage;
using Microsoft.EntityFrameworkCore;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Infrastructure.Postgres;
using SharedAbstractions.Queries;

namespace Optimus.Services.Customers.Infrastructure.Storage;

public class CustomerStorage : ICustomerStorage
{
    private readonly DbSet<Customer> _customers;

    public CustomerStorage(CustomersDbContext dbContext)
    {
        _customers = dbContext.Customers;
    }

    public Task<Customer?> FindAsync(Expression<Func<Customer, bool>> expression)
        => _customers
            .AsNoTracking()
            .AsQueryable()
            .Where(expression)
            .SingleOrDefaultAsync();

    public Task<Paged<Customer>> BrowseAsync(Expression<Func<Customer, bool>> expression, IPagedQuery query)
        => _customers
            .AsNoTracking()
            .AsQueryable()
            .Where(expression)
            .OrderBy(x => x.CreatedAt)
            .PaginateAsync(query);
}