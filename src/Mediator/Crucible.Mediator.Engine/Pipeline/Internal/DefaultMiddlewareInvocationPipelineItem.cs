using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    /// <summary>
    /// The <see cref="DefaultMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> is an implementation
    /// of <see cref="IMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> typed with <typeparamref name="TRequest"/> and
    /// <typeparamref name="TResponse"/>. It also implements <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// to be injected in an <see cref="IInvocationPipeline{TResponse}"/> easily.
    /// </summary>
    /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMiddlewareInvocationPipelineItem{TRequest, TResponse}"/>
    public class DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse> : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        private static readonly Type s_matchingInvocationContextType = typeof(IInvocationContext<TRequest, TResponse>);

        private readonly IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> _resolver;

        /// <inheritdoc/>
        public int Order { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="DefaultMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="order"><inheritdoc cref="IMiddlewareInvocationPipelineItem.Order" path="/summary"/></param>
        /// <param name="resolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationMiddleware{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null" />.</exception>
        public DefaultMiddlewareInvocationPipelineItem(int order, IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> resolver)
        {
            ArgumentNullException.ThrowIfNull(resolver, nameof(resolver));

            Order = order;
            _resolver = resolver;
        }

        /// <inheritdoc/>
        public bool IsApplicable(Type contextType)
        {
            return s_matchingInvocationContextType.IsAssignableFrom(contextType);
        }

        /// <inheritdoc/>
        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            var middleware = _resolver.ResolveComponent();
            return middleware.HandleAsync(
                context,
                next,
                cancellationToken);
        }
    }
}
