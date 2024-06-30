namespace Optimus.Services.Customers.Application.Services;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}