namespace Optimus.Services.Customers.Infrastructure.Postgres.MockData.Models;

public class Address
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string Zipcode { get; set; }
    public MockGeo MockGeo { get; set; }
}