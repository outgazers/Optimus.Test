namespace Optimus.Services.Identity.Core.Exceptions;

public class VerificationCodeTimeException : DomainException
{
    public override string Code { get; } = "verification_code_time_exception";
    public TimeSpan TimeToWait { get; }

    public VerificationCodeTimeException(TimeSpan timeToWait) : base($"Please wait for {timeToWait}.")
    {
        TimeToWait = timeToWait;
    }
}