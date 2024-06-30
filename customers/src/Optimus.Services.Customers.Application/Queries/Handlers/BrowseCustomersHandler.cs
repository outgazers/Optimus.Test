using System.Linq.Expressions;
using Convey.CQRS.Queries;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Application.DTO;
using Optimus.Services.Customers.Application.Storage;
using SharedAbstractions;
using SharedAbstractions.Queries;

namespace Optimus.Services.Customers.Application.Queries.Handlers;

public class BrowseCustomersHandler : IQueryHandler<BrowseCustomers, Paged<CustomerDto>>
{
    private readonly ICustomerStorage _customerStorage;

    public BrowseCustomersHandler(ICustomerStorage customerStorage)
    {
        _customerStorage = customerStorage;
    }

    public async Task<Paged<CustomerDto>> HandleAsync(BrowseCustomers query, CancellationToken cancellationToken = new CancellationToken())
    {
        Expression<Func<Customer, bool>> expression = x => true;

        if (query.IsVip.HasValue)
            expression = expression.And(x => x.IsVip == query.IsVip);
        
        if (!string.IsNullOrEmpty(query.Address))
            expression = expression.And(x => x.Address == query.Address);
        
        if (!string.IsNullOrEmpty(query.FullName))
            expression = expression.And(x => x.FullName == query.FullName);
        
        if (query.State.HasValue)
            expression = expression.And(x => x.State == query.State);
        
        if (!string.IsNullOrWhiteSpace(query.Email))
            expression = expression.And(x => x.Email == query.Email);
        
        if (!string.IsNullOrWhiteSpace(query.Username))
            expression = expression.And(x => x.Username == query.Username);
        
        var result = await _customerStorage.BrowseAsync(expression, query);
        var customers = result.Items.Select(x => x.AsDto()).ToList();
        
        return Paged<CustomerDto>.From(result, customers);
    }
}