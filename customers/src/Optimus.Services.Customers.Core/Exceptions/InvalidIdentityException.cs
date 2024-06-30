namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidIdentityException : DomainException
{
    public string Iban { get; }
    public long CardNumber { get; set; }

    public InvalidIdentityException(string iban, long cardNumber)
        : base($"Bank Account with iban: '{iban}', cardNumber: '{cardNumber}' is invalid.")
    {
        Iban = iban;
        CardNumber = cardNumber;
    }
}