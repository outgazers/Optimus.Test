using Convey.CQRS.Commands;
using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Application.Commands;

[Contract]
public class CompleteCustomerRegistrationFromUser : ICommand
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; }
    public string CompanyName { get; set; }
    public string LocationStateAndCity { get; set; }
    public string? MC { get; set; }
    public string PhoneNumber { get; set; } 
    public string? NetTerms { get; set; } 
    public string? TMS { get; set; } 
    public bool IsAssetBase { get; set; } 
    public List<ModesOfTransportation> ModesOfTransportation { get; set; }
    public string Industry { get; set; }
    public int YearsInBusiness { get; set; }
}