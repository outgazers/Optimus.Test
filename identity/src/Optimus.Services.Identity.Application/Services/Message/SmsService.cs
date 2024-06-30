using Convey.HTTP;
using Microsoft.Extensions.Logging;
using SmsIrRestful;

namespace Optimus.Services.Identity.Application.Services.Message;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly IHttpClient _httpClient;
    private readonly string _token;

    public SmsService(ILogger<SmsService> logger, IHttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _token = "new Token().GetToken(\"adabec0a221caaaaaaaff92d\", \"5555555\")";
    }
    public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code)
    {
        try
        {
            
            // Part of Send sms with third party
            //var restVerificationCodeResponse = new VerificationCode().Send(_token, restVerificationCode);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }
}