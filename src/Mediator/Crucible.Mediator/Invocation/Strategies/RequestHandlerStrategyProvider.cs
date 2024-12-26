using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Invocation.Strategies
{
    public class RequestHandlerStrategyProvider : IRequestHandlerStrategyProvider
    {
        private readonly IServiceProvider _services;

        public RequestHandlerStrategyProvider(IServiceProvider services)
        {
            _services = services;
        }

        public IRequestHandlerStrategy<TRequest, TResponse> GetRequestHandlerStrategyForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context)
        {
            return _services.GetRequiredService<IRequestHandlerStrategy<TRequest, TResponse>>();
        }
    }
}
