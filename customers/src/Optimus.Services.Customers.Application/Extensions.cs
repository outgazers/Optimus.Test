using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;

namespace Optimus.Services.Customers.Application;

public static class Extensions
{
    public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        => builder
            .AddQueryHandlers()
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddInMemoryQueryDispatcher()
            .AddInMemoryCommandDispatcher()
            .AddInMemoryEventDispatcher();
}