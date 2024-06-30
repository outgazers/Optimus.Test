using Convey.CQRS.Queries;
using Optimus.Services.Customers.Application.DTO;

namespace Optimus.Services.Customers.Application.Queries;

public class GetCustomer : IQuery<CustomerDetailsDto>
{
    public Guid CustomerId { get; set; }
}