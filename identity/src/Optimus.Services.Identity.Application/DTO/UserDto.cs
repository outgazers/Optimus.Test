using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Application.DTO;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? SendVerificationCodeAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<string> Permissions { get; set; }

    public UserDto()
    {
    }

    public UserDto(User user)
    {
        Id = user.Id;
        Email = user.Email;
        Username = user.Username;
        Role = user.Role;
        IsVerified = user.IsVerified;
        SendVerificationCodeAt = user.SentVerificationCodeAt;
        CreatedAt = user.CreatedAt;
        UpdatedAt = user.UpdatedAt;
        Permissions = user.Permissions;
    }
}