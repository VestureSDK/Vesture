using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.Engine.Pipeline.Strategies
{
    /// <summary>
    /// The <see cref="SequentialHandlersStrategy{TRequest, TResponse}"/> is a <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// invoking the <see cref="IInvocationHandler{TRequest, TResponse}"/> instances squentially and awaiting the current one
    /// before invoking the next one.
    /// </summary>
    /// <inheritdoc cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// <seealso cref="ParallelHandlersStrategy{TRequest, TResponse}"/>
    public class SequentialHandlersStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> _resolvers;

        /// <summary>
        /// Initializes a new <see cref="SequentialHandlersStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="resolvers">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationHandler{TRequest, TResponse}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolvers"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="resolvers"/> is empty.</exception>
        public SequentialHandlersStrategy(IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> resolvers)
        {
            ArgumentNullException.ThrowIfNull(resolvers, nameof(resolvers));
            if (!resolvers.Any())
            {
                throw new ArgumentException($"{nameof(resolvers)} is empty", nameof(resolvers));
            }

            _resolvers = resolvers;
        }

        /// <inheritdoc />
        public async Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            foreach (var resolver in _resolvers)
            {
                await SingleHandlerStrategy<TRequest, TResponse>.InvokeHandlerAsync(resolver, context, cancellationToken);
            }
        }
    }
}
