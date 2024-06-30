namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidAddressException : DomainException
{
    public string Address { get; }

    public InvalidAddressException(string address) : base($"Address: '{address}' is invalid.")
    {
        Address = address;
    }
}