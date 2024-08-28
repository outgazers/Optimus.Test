using Convey.CQRS.Events;

namespace Optimus.Services.Identity.Application.Events;

[Contract]
public class SignedUp : IEvent
{
    public Guid UserId { get; }
    public string Username { get; set; }
    public string Email { get; }
    public string Role { get; }
    public int CrmAccountId { get; }
    
    public SignedUp(Guid userId, string email, string username, string role, int crmAccountId)
    {
        UserId = userId;
        Username = username;
        Email = email;
        Role = role;
        CrmAccountId = crmAccountId;
    }
}