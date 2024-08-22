using Convey.CQRS.Queries;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Application.DTO;

namespace Optimus.Services.Customers.Application.Queries.Handlers;

public class GetCustomerForUserHandler : IQueryHandler<GetCustomerForUser, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAppContext _appContext;

    public GetCustomerForUserHandler(ICustomerRepository customerRepository, IAppContext appContext)
    {
        _customerRepository = customerRepository;
        _appContext = appContext;
    }

    public async Task<CustomerDetailsDto> HandleAsync(GetCustomerForUser query, CancellationToken cancellationToken = new CancellationToken())
    {
        var customer = await _customerRepository.GetAsync(_appContext.Identity.Id);

        return new CustomerDetailsDto
        {
            Id = customer.Id,
            Address = customer.Address,
            State = customer.State.ToString(),
            FullName = customer.FullName,
            IsVip = customer.IsVip,
            Email = customer.Email,
            Username = customer.Username,
        };
    }
}