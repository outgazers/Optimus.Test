using Convey.CQRS.Commands;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Exceptions;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Application.Commands.Handlers;

public class ChangeCustomerStateHandler : ICommandHandler<ChangeCustomerState>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IMessageBroker _messageBroker;

    public ChangeCustomerStateHandler(ICustomerRepository customerRepository, IEventMapper eventMapper,
        IMessageBroker messageBroker)
    {
        _customerRepository = customerRepository;
        _eventMapper = eventMapper;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ChangeCustomerState command, CancellationToken cancellationToken = new CancellationToken())
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }

        if (!Enum.TryParse<State>(command.State, true, out var state))
        {
            throw new CannotChangeCustomerStateException(customer.Id, State.Unknown);
        }

        if (customer.State == state)
        {
            return;
        }

        switch (state)
        {
            case State.Valid:
                customer.SetValid();
                break;
            case State.Incomplete:
                customer.SetIncomplete();
                break;
            case State.Suspicious:
                customer.MarkAsSuspicious();
                break;
            case State.Locked:
                customer.Lock();
                break;
            default:
                throw new CannotChangeCustomerStateException(customer.Id, state);
        }

        await _customerRepository.UpdateAsync(customer);
        var events = _eventMapper.MapAll(customer.Events);
        await _messageBroker.PublishAsync(events.ToArray());
    }
}