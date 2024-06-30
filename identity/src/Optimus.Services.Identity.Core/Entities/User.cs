namespace Optimus.Services.Identity.Core.Entities;

public class User : AggregateRoot
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string Password { get; set; }
    public bool IsVerified { get; set; }
    public string VerificationCode { get; set; }
    public SignUpState SignUpState { get; set; }
    public DateTime? SentVerificationCodeAt { get; set; }
    public int? SentVerificationCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<string> Permissions { get; set; }
}
