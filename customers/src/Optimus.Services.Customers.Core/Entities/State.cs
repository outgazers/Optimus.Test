namespace Optimus.Services.Customers.Core.Entities
{
    public enum State
    {
        Unknown,
        AwaitForValidate,
        Valid,
        Incomplete,
        Suspicious,
        Locked
    }
}