namespace Optimus.Services.Customers.Application;

public interface IIdentityContext
{
    Guid Id { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    IDictionary<string, string> Claims { get; }
}