namespace Optimus.Services.Customers.Application.Exceptions;

public class InvalidCustomerStateException : AppException
{
    public override string Code { get; } = "invalid_customer_state_exception";
    public Guid Id { get; }
    
    public InvalidCustomerStateException(Guid id) 
        : base($"Customer with id: {id} has invalid state.")
    {
        Id = id;
    }
}