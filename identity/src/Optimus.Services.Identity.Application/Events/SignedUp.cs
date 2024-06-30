using Convey.CQRS.Events;

namespace Optimus.Services.Identity.Application.Events;

[Contract]
public class SignedUp : IEvent
{
    public Guid UserId { get; }
    public string Username { get; set; }
    public string Email { get; }
    public string Role { get; }
    
    public SignedUp(Guid userId, string email, string username, string role)
    {
        UserId = userId;
        Username = username;
        Email = email;
        Role = role;
    }
}