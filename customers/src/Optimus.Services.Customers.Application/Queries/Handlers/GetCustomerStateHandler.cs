using Convey.CQRS.Queries;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Application.DTO;

namespace Optimus.Services.Customers.Application.Queries.Handlers;

public class GetCustomerStateHandler : IQueryHandler<GetCustomerState, CustomerStateDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerStateHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerStateDto> HandleAsync(GetCustomerState query, CancellationToken cancellationToken = new CancellationToken())
    {
        var document = await _customerRepository.GetAsync(query.CustomerId);

        return document is null
            ? null
            : new CustomerStateDto
            {
                Id = document.Id,
                State = document.State.ToString().ToLowerInvariant()
            };
    }
}