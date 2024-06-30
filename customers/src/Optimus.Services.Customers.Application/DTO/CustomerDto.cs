namespace Optimus.Services.Customers.Application.DTO;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string State { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}