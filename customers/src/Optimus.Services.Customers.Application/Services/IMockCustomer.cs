using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Application.Services;

public interface IMockCustomer
{
    Task InitializeCustomers(List<Customer> customers);
}