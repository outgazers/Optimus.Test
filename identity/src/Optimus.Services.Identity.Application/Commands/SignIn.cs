using Convey.CQRS.Commands;

namespace Optimus.Services.Identity.Application.Commands;

[Contract]
public class SignIn : ICommand
{
    public string Password { get; set; }
    public string Email { get; set; }

    public SignIn(string password, string email)
    {
        Email = email;
        Password = password;
    }
}