namespace Optimus.Services.Identity.Core.Exceptions;

public class InvalidVerificationCodeException : DomainException
{
    public override string Code { get; } = "invalid_verification_code";
        
    public InvalidVerificationCodeException() : base($"Invalid verification code ")
    {
    }
}