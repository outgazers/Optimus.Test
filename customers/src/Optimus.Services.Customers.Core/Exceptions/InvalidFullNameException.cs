namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidFullNameException : DomainException
{
    public string FullName { get; }

    public InvalidFullNameException(string fullName) : base($"FullName: '{fullName}' is invalid.")
    {
        FullName = fullName;
    }
}