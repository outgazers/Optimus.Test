using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Infrastructure.Postgres.MockData.Models;

namespace Optimus.Services.Customers.Infrastructure.Postgres.MockData;

public static class MockCustomerMapper
{
    public static Customer MapToCustomer(MockCustomer mockCustomer)
    {
        return new Customer(
            Guid.NewGuid(),
            mockCustomer.Email,
            DateTime.UtcNow,
            DateTime.UtcNow,
            mockCustomer.Name,
            $"{mockCustomer.Address.Street}, {mockCustomer.Address.Suite}, {mockCustomer.Address.City}, {mockCustomer.Address.Zipcode}",
            false,
            State.Valid,
            mockCustomer.Username
        );
    }
}