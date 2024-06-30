namespace Optimus.Services.Customers.Infrastructure.Postgres.MockData.Models;

public class MockCustomer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public MockCompany MockCompany { get; set; }
}