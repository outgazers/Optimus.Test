using Convey.CQRS.Events;
using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Application.Events;

[Contract]
public class CustomerCreated : IEvent
{
    public Guid CustomerId { get; }
    public List<ModesOfTransportation> ModsOfTransportation { get; }
    public string Industry { get; }
    public string Address { get; }

    public CustomerCreated(Guid customerId, List<ModesOfTransportation> modsOfTransportation, string industry, string address)
    {
        CustomerId = customerId;
        ModsOfTransportation = modsOfTransportation;
        Industry = industry;
        Address = address;
    }
}