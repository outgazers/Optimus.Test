namespace Optimus.Services.Identity.Core.Entities;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
    public IEnumerable<IDomainEvent> Events => _events;
    public AggregateId Id { get; set; }
    public int Version { get; protected set; }

    protected void AddEvent(IDomainEvent @event)
    {
        _events.Add(@event);
    }

    public void ClearEvents() => _events.Clear();
}