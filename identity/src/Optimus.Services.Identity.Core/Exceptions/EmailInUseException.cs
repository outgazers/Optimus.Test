namespace Optimus.Services.Identity.Core.Exceptions;

public class EmailInUseException : DomainException
{
    public override string Code { get; } = "phone_number_in_use";
    public string Email { get; }

    public EmailInUseException(string email) : base($"Email {email} is already in use.")
    {
        Email = email;
    }
}