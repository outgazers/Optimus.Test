using Convey.CQRS.Commands;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Exceptions;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Core.ValueObjects;
using Optimus.Services.Customers.Application.Exceptions;
using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Application.Commands.Handlers;

public class CompleteCustomerRegistrationHandler : ICommandHandler<CompleteCustomerRegistration>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IMessageBroker _messageBroker;
    
    public CompleteCustomerRegistrationHandler(ICustomerRepository customerRepository, IEventMapper eventMapper,
        IMessageBroker messageBroker)
    {
        _customerRepository = customerRepository;
        _eventMapper = eventMapper;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(CompleteCustomerRegistration command, CancellationToken cancellationToken = new CancellationToken())
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }
        
        if (customer.State is State.Valid)
        {
            throw new CustomerAlreadyRegisteredException(command.CustomerId);
        }

        //customer.CompleteRegistration(command.FullName, command.Address, command.BirthDate, command.NationalCode);
        await _customerRepository.UpdateAsync(customer);

        var events = _eventMapper.MapAll(customer.Events);
        await _messageBroker.PublishAsync(events.ToArray());
    }
}