using Convey.CQRS.Events;

namespace Optimus.Services.Identity.Application.Services;

public interface IMessageBroker
{
    Task PublishAsync(params IEvent[] events);
}