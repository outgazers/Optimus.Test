namespace Optimus.Services.Identity.Core.Exceptions;

public class ExceededTryVerifyCode : DomainException
{
    public override string Code { get; } = "exceeded_try_verify_code";
    
    public ExceededTryVerifyCode() : base("Exceeded try verify code.")
    {
    }
}