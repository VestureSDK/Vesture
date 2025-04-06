using Microsoft.Extensions.Logging;
using Vesture.Mediator.Engine.Pipeline.Resolvers;
using Vesture.Mediator.Events;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Pipeline.Strategies
{
    /// <summary>
    /// The <see cref="ParallelHandlersStrategy{TRequest, TResponse}"/> is a <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// invoking the <see cref="IInvocationHandler{TRequest, TResponse}"/> instances in parallel and awaiting all of their
    /// invocation with <c>Task.WhenAll</c>.
    /// <para>
    /// It is the default <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> for <see cref="IEvent"/>.
    /// </para>
    /// </summary>
    /// <inheritdoc cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// <seealso cref="SequentialHandlersStrategy{TRequest, TResponse}"/>
    public class ParallelHandlersStrategy<TRequest, TResponse>
        : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        private readonly IEnumerable<
            IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>
        > _resolvers;

        /// <summary>
        /// Initializes a new <see cref="ParallelHandlersStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
        /// <param name="resolvers">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationHandler{TRequest, TResponse}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" /> or <paramref name="resolvers"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="resolvers"/> is empty.</exception>
        public ParallelHandlersStrategy(
            ILogger<ParallelHandlersStrategy<TRequest, TResponse>> logger,
            IEnumerable<
                IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>
            > resolvers
        )
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (resolvers is null)
            {
                throw new ArgumentNullException(nameof(resolvers));
            }

            if (!resolvers.Any())
            {
                throw new ArgumentException($"{nameof(resolvers)} is empty", nameof(resolvers));
            }

            _logger = logger;
            _resolvers = resolvers;
        }

        /// <inheritdoc />
        public Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        )
        {
            var tasks = _resolvers.Select(resolver =>
                SingleHandlerStrategy<TRequest, TResponse>.InvokeHandlerAsync(
                    _logger,
                    resolver,
                    context,
                    cancellationToken
                )
            );
            return Task.WhenAll(tasks);
        }
    }
}
