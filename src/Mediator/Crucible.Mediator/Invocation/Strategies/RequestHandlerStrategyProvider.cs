using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// Default implementation of <see cref="IRequestHandlerStrategyProvider"/>.
    /// </summary>
    public class RequestHandlerStrategyProvider : IRequestHandlerStrategyProvider
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new <see cref="RequestHandlerStrategyProvider"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/> instance.</param>
        public RequestHandlerStrategyProvider(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc/>
        public IRequestHandlerStrategy<TRequest, TResponse> GetRequestHandlerStrategyForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context)
        {
            return _services.GetRequiredService<IRequestHandlerStrategy<TRequest, TResponse>>();
        }
    }
}
