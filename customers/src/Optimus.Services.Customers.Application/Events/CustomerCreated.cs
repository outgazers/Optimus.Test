using Convey.CQRS.Events;
using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Application.Events;

[Contract]
public class CustomerCreated : IEvent
{
    public Guid CustomerId { get; }
    public string Email { get; }
    public int CrmAccountId { get; }
    public string CrmToken { get; }
    public List<ModesOfTransportation> ModsOfTransportation { get; }
    public string Industry { get; }
    public string Address { get; }

    public CustomerCreated(Guid customerId, int crmAccountId, List<ModesOfTransportation> modsOfTransportation, string industry, string address, string crmToken, string email)
    {
        CustomerId = customerId;
        ModsOfTransportation = modsOfTransportation;
        Industry = industry;
        Address = address;
        CrmAccountId = crmAccountId;
        CrmToken = crmToken;
        Email = email;
    }
}