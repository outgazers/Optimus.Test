using Convey.CQRS.Commands;

namespace Optimus.Services.Identity.Application.Commands;

public class RevokeAccessToken : ICommand
{
    public string AccessToken { get; }

    public RevokeAccessToken(string accessToken)
    {
        AccessToken = accessToken;
    }
}