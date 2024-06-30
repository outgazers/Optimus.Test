using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.WebApi.CQRS;

namespace Optimus.Services.Identity.Infrastructure.Dispatchers;

internal sealed class InMemoryDispatcher : IDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public InMemoryDispatcher(ICommandDispatcher commandDispatcher, IEventDispatcher eventDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _eventDispatcher = eventDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    public Task SendAsync<T>(T command, CancellationToken cancellationToken = new CancellationToken()) where T : class, ICommand
        => _commandDispatcher.SendAsync(command);

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        => _eventDispatcher.PublishAsync(@event);

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = new CancellationToken())
        => _queryDispatcher.QueryAsync(query);
}