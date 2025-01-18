using Ingot.Mediator.Engine.Pipeline.Extensions;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Microsoft.Extensions.Logging;

namespace Ingot.Mediator.Engine.Pipeline.Internal.NoOp
{
    /// <summary>
    /// Default implementation of <see cref="INoOpInvocationHandlerStrategyResolver"/>.
    /// </summary>
    /// <seealso cref="INoOpInvocationHandlerStrategyResolver"/>
    public class DefaultNoOpInvocationHandlerStrategyResolver : INoOpInvocationHandlerStrategyResolver
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new <see cref="DefaultNoOpInvocationHandlerStrategyResolver"/> instance.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="loggerFactory"/> is <see langword="null"/>.</exception>
        public DefaultNoOpInvocationHandlerStrategyResolver(ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));

            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IInvocationHandlerStrategy<object, TResponse> ResolveNoOpInvocationHandlerStrategy<TResponse>()
        {
            var logger = _loggerFactory.CreateLogger<NoOpInvocationHandlerStrategy<TResponse>>();
            return new NoOpInvocationHandlerStrategy<TResponse>(logger);
        }

        private class NoOpInvocationHandlerStrategy<TResponse> : IInvocationHandlerStrategy<object, TResponse>
        {
            private readonly ILogger _logger;

            public NoOpInvocationHandlerStrategy(ILogger<NoOpInvocationHandlerStrategy<TResponse>> logger)
            {
                _logger = logger;
            }

            public Task HandleAsync(IInvocationContext<object, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
            {
                if (typeof(TResponse) != EventResponse.Type)
                {
                    var error = new KeyNotFoundException($"No relevant invocation pipeline found for contract '{context.RequestType.Name} -> {typeof(TResponse).Name}'.");
                    context.AddError(error);

                    _logger.NoHandlersRegisteredException(context, error);
                }

                return Task.CompletedTask;
            }
        }
    }
}
