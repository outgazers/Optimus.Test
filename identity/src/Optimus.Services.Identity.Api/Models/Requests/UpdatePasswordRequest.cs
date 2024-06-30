namespace Optimus.Services.Identity.Api.Models.Requests;

public class UpdatePasswordRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string VerificationCode { get; set; }
}