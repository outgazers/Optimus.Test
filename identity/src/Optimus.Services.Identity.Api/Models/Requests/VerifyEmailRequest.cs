namespace Optimus.Services.Identity.Api.Models.Requests;

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}