namespace Optimus.Services.Identity.Application.Services;

public interface ISmsService
{
    Task<bool> SendVerificationCodeAsync(string phoneNumber, string code);
}