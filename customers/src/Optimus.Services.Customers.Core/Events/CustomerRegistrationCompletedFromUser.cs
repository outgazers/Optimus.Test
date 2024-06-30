using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Core.Events;

public class CustomerRegistrationCompletedFromUser : IDomainEvent
{
    public Customer Customer { get; }

    public CustomerRegistrationCompletedFromUser(Customer customer)
    {
        Customer = customer;
    }
}