using Convey.CQRS.Commands;
using Convey.Types;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Decorators;

[Decorator]
public class TransactionalCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandHandler<T> _handler;
    private readonly ILogger<TransactionalCommandHandlerDecorator<T>> _logger;

    public TransactionalCommandHandlerDecorator(ICommandHandler<T> handler,
        IUnitOfWork unitOfWork, ILogger<TransactionalCommandHandlerDecorator<T>> logger)
    {
        _handler = handler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        var name = command.GetType().Name.Underscore();
        _logger.LogInformation("Handling: {Name} using TX (UnitOfWorkName)...", name);
        await _unitOfWork.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken));
        _logger.LogInformation("Handled: {Name} using TX (UnitOfWorkName)", name);
    }
}