namespace Optimus.Services.Identity.Application.Exceptions;

public class IsNotVerifiedException : AppException
{
    public string Email { get; }
    
    public IsNotVerifiedException(string email) : base($"User with Email: '{email}' was not verified.")
    {
        Email = email;
    }
}