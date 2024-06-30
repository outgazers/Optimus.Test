namespace Optimus.Services.Customers.Application;

public interface IAppContext
{
    string RequestId { get; }
    IIdentityContext Identity { get; }
}