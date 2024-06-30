using Convey.CQRS.Commands;
using Optimus.Services.Customers.Core.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Optimus.Services.Customers.Application.Commands;

[Contract]
public class CompleteCustomerRegistration : ICommand
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string NationalCode { get; set; }
    public string Address { get; set; }
}