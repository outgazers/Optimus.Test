using Convey;
using Convey.Logging.CQRS;
using Optimus.Services.Customers.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Optimus.Services.Customers.Infrastructure.Logging;

internal static class Extensions
{
    public static IConveyBuilder AddHandlersLogging(this IConveyBuilder builder)
    {
        var assembly = typeof(CompleteCustomerRegistration).Assembly;
        
        builder.Services.AddSingleton<IMessageToLogTemplateMapper>(new MessageToLogTemplateMapper());
        
        return builder
            .AddCommandHandlersLogging(assembly)
            .AddEventHandlersLogging(assembly);
    }
}