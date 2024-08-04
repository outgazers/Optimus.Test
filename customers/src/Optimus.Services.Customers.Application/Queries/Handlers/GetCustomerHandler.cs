using Convey.CQRS.Queries;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Application.DTO;
using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Application.Queries.Handlers;

public class GetCustomerHandler : IQueryHandler<GetCustomer, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IFileManager _fileManager;

    public GetCustomerHandler(ICustomerRepository customerRepository, IFileManager fileManager)
    {
        _customerRepository = customerRepository;
        _fileManager = fileManager;
    }

    public async Task<CustomerDetailsDto> HandleAsync(GetCustomer query, CancellationToken cancellationToken = new CancellationToken())
    {
        var customer = await _customerRepository.GetAsync(query.CustomerId);

        return new CustomerDetailsDto
        {
            Address = customer.Address,
            Email = customer.Email,
            Username = customer.Username,
            Id = customer.Id,
            State = customer.State.ToString(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            FullName = customer.FullName,
            IsVip = customer.IsVip,
        };
    }
}