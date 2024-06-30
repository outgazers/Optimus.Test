namespace Optimus.Services.Identity.Core.Exceptions;

public class SendSmsWasNotSucceeded : DomainException
{
    public override string Code { get; } = "send_sms_was_not_succeeded";
    
    public SendSmsWasNotSucceeded() : base("Send sms was not succeeded.")
    {
    }
}

public class SendEmailWasNotSucceeded : DomainException
{
    public override string Code { get; } = "send_email_was_not_succeeded";
    
    public SendEmailWasNotSucceeded() : base("Send email was not succeeded.")
    {
    }
}