using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;

namespace Optimus.Services.Identity.Application;

public static class Extensions
{
    public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        => builder
            .AddQueryHandlers()
            .AddInMemoryQueryDispatcher()
            .AddCommandHandlers()
            .AddEventHandlers()
            .AddInMemoryCommandDispatcher()
            .AddInMemoryEventDispatcher();
}