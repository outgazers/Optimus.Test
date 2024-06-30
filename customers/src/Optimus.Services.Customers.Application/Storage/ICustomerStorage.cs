using System.Linq.Expressions;
using Optimus.Services.Customers.Core.Entities;
using SharedAbstractions.Queries;

namespace Optimus.Services.Customers.Application.Storage;

public interface ICustomerStorage
{
    Task<Customer> FindAsync(Expression<Func<Customer, bool>> expression);
    Task<Paged<Customer>> BrowseAsync(Expression<Func<Customer, bool>> expression, IPagedQuery query);
}