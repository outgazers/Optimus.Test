namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidModsOfTransportation : DomainException
{
    public override string Code { get; } = "invalid_customer_mods_of_transportation";
    public Guid Id { get; }

    public InvalidModsOfTransportation(Guid id) : base(
        $"Customer with id: {id} has invalid mods of transportation.")
    {
        Id = id;
    }
}