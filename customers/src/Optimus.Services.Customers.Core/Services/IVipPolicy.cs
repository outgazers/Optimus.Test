using Optimus.Services.Customers.Core.Entities;

namespace  Optimus.Services.Customers.Core.Services;

public interface IVipPolicy
{
    void ApplyVipStatusIfEligible(Customer customer);
}