namespace Optimus.Services.Identity.Core.Exceptions;

public class InvalidRoleException : DomainException
{
    public override string Code { get; } = "invalid_role";
    
    public InvalidRoleException(string role) : base($"Invalid role: {role}.")
    {
    }
}