using Convey.CQRS.Events;
using Convey.MessageBrokers;

namespace Optimus.Services.Customers.Application.Events.External;

[Message("identity")]
public class SignedUp : IEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; set; }
    public string Role { get; }
    
    public SignedUp(Guid userId, string email, string username, string role)
    {
        UserId = userId;
        Email = email;
        Username = username;
        Role = role;
    }
}