using Convey.CQRS.Events;

namespace Optimus.Services.Customers.Application.Services;

public interface IMessageBroker
{
    Task PublishAsync(params IEvent[] events);
    Task PublishAsync(IEnumerable<IEvent> events);
}