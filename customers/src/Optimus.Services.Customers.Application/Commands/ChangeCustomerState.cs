using Convey.CQRS.Commands;

namespace Optimus.Services.Customers.Application.Commands;

[Contract]
public class ChangeCustomerState : ICommand
{
    public Guid CustomerId { get; }
    public string State { get; }

    public ChangeCustomerState(Guid customerId, string state)
    {
        CustomerId = customerId;
        State = state;
    }
}