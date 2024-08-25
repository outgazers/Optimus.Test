using Convey.CQRS.Commands;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Exceptions;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Core.ValueObjects;
using Optimus.Services.Customers.Application.Exceptions;
using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Application.Commands.Handlers;

public class CompleteCustomerRegistrationFromUserHandler : ICommandHandler<CompleteCustomerRegistrationFromUser>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IMessageBroker _messageBroker;
    private readonly IAppContext _appContext;

    public CompleteCustomerRegistrationFromUserHandler(ICustomerRepository customerRepository, IEventMapper eventMapper,
        IMessageBroker messageBroker, IFileManager fileManager, IAppContext appContext)
    {
        _customerRepository = customerRepository;
        _eventMapper = eventMapper;
        _messageBroker = messageBroker;
        _appContext = appContext;
    }

    public async Task HandleAsync(CompleteCustomerRegistrationFromUser command, CancellationToken cancellationToken = new CancellationToken())
    {
        var customer = await _customerRepository.GetAsync(_appContext.Identity.Id);
        if (customer is null)
        {
            throw new CustomerNotFoundException(_appContext.Identity.Id);
        }
        
        if (customer.State is State.Valid)
        {
            throw new CustomerAlreadyRegisteredException(_appContext.Identity.Id);
        }

        if (customer.State is State.AwaitForValidate)
        {
            throw new InvalidCustomerStateException(_appContext.Identity.Id);
        }

        customer.CompleteRegistrationFromUser(command.FullName, command.LocationStateAndCity, command.CompanyName,
            command.MC, command.PhoneNumber, command.NetTerms, command.TMS, command.IsAssetBase,
            command.ModesOfTransportation, command.Industry, command.YearsInBusiness);
        await _customerRepository.UpdateAsync(customer);

        var events = _eventMapper.MapAll(customer.Events);
        if(events.Any())
            await _messageBroker.PublishAsync(events.ToArray());
    }
}