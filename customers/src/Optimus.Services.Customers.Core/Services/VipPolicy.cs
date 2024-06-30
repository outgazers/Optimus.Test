using Optimus.Services.Customers.Core.Entities;

namespace Optimus.Services.Customers.Core.Services;

public class VipPolicy : IVipPolicy
{
    public void ApplyVipStatusIfEligible(Customer customer)
    {
        if (customer.IsVip)
        {
            return;
        }
        
        customer.SetVip();
    }
}