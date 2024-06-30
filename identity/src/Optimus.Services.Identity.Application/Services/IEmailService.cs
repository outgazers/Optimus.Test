namespace Optimus.Services.Identity.Application.Services;

public interface IEmailService
{
    Task<bool> SendVerificationCodeAsync(string email, string code);
}