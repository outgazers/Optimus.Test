using Convey.CQRS.Events;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Repositories;
using Microsoft.Extensions.Logging;
using Optimus.Services.Customers.Application.Exceptions;
using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Application.Events.External.Handlers;

public class SignedUpHandler : IEventHandler<SignedUp>
{
    private const string RequiredRole = "user";
    private readonly ICustomerRepository _customerRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<SignedUpHandler> _logger;

    public SignedUpHandler(ICustomerRepository customerRepository, IDateTimeProvider dateTimeProvider,
        ILogger<SignedUpHandler> logger)
    {
        _customerRepository = customerRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task HandleAsync(SignedUp @event, CancellationToken cancellationToken = new CancellationToken())
    {
        if (@event.Role != RequiredRole)
        {
            throw new InvalidRoleException(@event.UserId, @event.Role, RequiredRole);
        }

        var customer = await _customerRepository.GetAsync(@event.UserId);
        if (customer is not null)
        {
            throw new CustomerAlreadyCreatedException(customer.Id);
        }

        customer = new Customer(@event.UserId, @event.Email, _dateTimeProvider.Now, _dateTimeProvider.Now, @event.Username);
        await _customerRepository.AddAsync(customer);
    }
}