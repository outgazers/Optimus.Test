using Optimus.Services.Customers.Application.Services;

namespace Optimus.Services.Customers.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now  => DateTime.UtcNow;
}