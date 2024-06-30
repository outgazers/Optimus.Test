namespace Optimus.Services.Identity.Application.Exceptions;

public class UserNotFoundException : AppException
{
    public override string Code { get; } = "user_not_found";
    public string Email { get; }
    public Guid UserId { get; }
    
    public UserNotFoundException(string email) : base($"User with Email: '{email}' was not found.")
    {
        Email = email;
    }
    
    public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
    {
        UserId = userId;
    }
    
}