using Convey.CQRS.Events;
using Optimus.Services.Customers.Core.Events;

namespace Optimus.Services.Customers.Application.Services;
public interface IEventMapper
{
    IEvent Map(IDomainEvent @event);
    IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events);
}