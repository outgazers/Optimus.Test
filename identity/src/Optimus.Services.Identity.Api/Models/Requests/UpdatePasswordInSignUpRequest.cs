namespace Optimus.Services.Identity.Api.Models.Requests;

public class UpdatePasswordInSignUpRequest
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}