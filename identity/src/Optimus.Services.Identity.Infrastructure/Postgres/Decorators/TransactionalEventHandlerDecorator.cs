using Convey.CQRS.Events;
using Convey.Types;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Decorators;

[Decorator]
public class TransactionalEventHandlerDecorator<T> : IEventHandler<T> where T : class, IEvent
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventHandler<T> _handler;
    private readonly ILogger<TransactionalEventHandlerDecorator<T>> _logger;

    public TransactionalEventHandlerDecorator(IEventHandler<T> handler,
        IUnitOfWork unitOfWork, ILogger<TransactionalEventHandlerDecorator<T>> logger)
    {
        _handler = handler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var name = @event.GetType().Name.Underscore();
        _logger.LogInformation("Handling: {Name} using TX (UnitOfWork)...", name);
        await _unitOfWork.ExecuteAsync(() => _handler.HandleAsync(@event, cancellationToken));
        _logger.LogInformation("Handled: {Name} using TX (UnitOfWorkName)", name);
    }
}