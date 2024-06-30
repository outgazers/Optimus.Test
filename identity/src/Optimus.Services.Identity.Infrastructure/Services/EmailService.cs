using Microsoft.Extensions.Logging;
using Optimus.Services.Identity.Application.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Optimus.Services.Identity.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _apiKey;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
        _apiKey = "";
    }

    public async Task<bool> SendVerificationCodeAsync(string email, string code)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("hexstartech.bot.noreply@gmail.com", "Morteza");
            var subject = "Verification Code";
            var to = new EmailAddress(email);
            var plainTextContent = $"Your verification code is: {code}";
            var htmlContent = $"<strong>Your verification code is: {code}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }
}