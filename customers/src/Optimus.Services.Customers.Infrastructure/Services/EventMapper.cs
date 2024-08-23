using Convey.CQRS.Events;
using Optimus.Services.Customers.Application.Services;
using Optimus.Services.Customers.Core.Events;

namespace Optimus.Services.Customers.Infrastructure.Services;

public class EventMapper : IEventMapper
{
    public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent Map(IDomainEvent @event)
    {
        switch (@event)
        {
            case CustomerRegistrationCompletedFromUser e: return new Optimus.Services.Customers.Application.Events.CustomerCreated(e.Customer.Id, e.Customer.ModesOfTransportation, e.Customer.Industry, e.Customer.Address);
            case CustomerBecameVip e: return new Optimus.Services.Customers.Application.Events.CustomerBecameVip(e.Customer.Id);
            case CustomerStateChanged e:
                return new Optimus.Services.Customers.Application.Events.CustomerStateChanged(e.Customer.Id,
                    e.Customer.State.ToString().ToLowerInvariant(), e.PreviousState.ToString().ToLowerInvariant());
        }

        return null;
    }
}