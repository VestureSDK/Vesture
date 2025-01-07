using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Strategies
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
    public class ParallelHandlersStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> _resolvers;

        /// <summary>
        /// Initializes a new <see cref="ParallelHandlersStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="resolvers">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationHandler{TRequest, TResponse}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolvers"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="resolvers"/> is empty.</exception>
        public ParallelHandlersStrategy(IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> resolvers)
        {
            ArgumentNullException.ThrowIfNull(resolvers, nameof(resolvers));
            if (!resolvers.Any())
            {
                throw new ArgumentException($"{nameof(resolvers)} is empty", nameof(resolvers));
            }

            _resolvers = resolvers;
        }

        /// <inheritdoc />
        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            var tasks = _resolvers.Select(resolver => SingleHandlerStrategy<TRequest, TResponse>.InvokeHandlerAsync(resolver, context, cancellationToken));
            return Task.WhenAll(tasks);
        }
    }
}
