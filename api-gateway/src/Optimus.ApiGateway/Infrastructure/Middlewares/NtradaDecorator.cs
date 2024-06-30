using Exchange.ApiGateway.Infrastructure.Decorators;
using Ntrada;

namespace Exchange.APIGateway.Infrastructure.Middlewares;

public static class NtradaDecorator
{
    public static IApplicationBuilder RegisterRequestHandlerManager(this IApplicationBuilder app)
    {
        var requestHandlerManager = app.ApplicationServices.GetRequiredService<IRequestHandlerManager>();
        requestHandlerManager.AddHandler("downstream2",
            app.ApplicationServices.GetRequiredService<DownstreamHandlerDecorator>());

        return app;
    }
    
}