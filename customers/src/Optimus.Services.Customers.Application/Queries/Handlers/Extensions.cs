using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Application.DTO;

namespace Optimus.Services.Customers.Application.Queries.Handlers;

public static class Extensions
{
    public static CustomerDto AsDto(this Customer customer)
        => customer.Map<CustomerDto>();
    
    private static T Map<T>(this Customer customer) where T : CustomerDto, new()
        => new()
        {
            Id = customer.Id,
            Email = customer.Email,
            Username = customer.Username,
            State = customer.State.ToString(),
            UpdatedAt = customer.UpdatedAt,
            CreatedAt = customer.CreatedAt,
        };
}