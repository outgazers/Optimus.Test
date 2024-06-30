using Ntrada;

namespace Exchange.ApiGateway.Infrastructure.Decorators;

public class RequestHandlerManagerDecorator : IRequestHandlerManager
{
    private readonly IRequestHandlerManager _requestHandlerManager;

    public RequestHandlerManagerDecorator(IRequestHandlerManager requestHandlerManager)
    {
        _requestHandlerManager = requestHandlerManager;
    }

    public IHandler Get(string name) => _requestHandlerManager.Get(name);

    public void AddHandler(string name, IHandler handler)
    {
        
        if (name.Equals("downstream"))
        {
        }
        else if(name.Equals("downstream2"))
        {
            _requestHandlerManager.AddHandler("downstream", handler);
        }
        else
            _requestHandlerManager.AddHandler(name, handler);
    }

    public async Task HandleAsync(string handler, HttpContext context, RouteConfig routeConfig) =>
        await _requestHandlerManager.HandleAsync(handler, context, routeConfig);
}