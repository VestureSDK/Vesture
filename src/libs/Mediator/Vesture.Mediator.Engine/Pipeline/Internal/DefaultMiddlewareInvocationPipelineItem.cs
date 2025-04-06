using Vesture.Mediator.Engine.Pipeline.Resolvers;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Pipeline.Internal
{
    /// <summary>
    /// The <see cref="DefaultMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> is an implementation
    /// of <see cref="IMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> typed with <typeparamref name="TRequest"/> and
    /// <typeparamref name="TResponse"/>. It also implements <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// to be injected in an <see cref="IInvocationPipeline{TResponse}"/> easily.
    /// </summary>
    /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMiddlewareInvocationPipelineItem{TRequest, TResponse}"/>
    public class DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>
        : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        private static readonly Type s_matchingInvocationContextType = typeof(IInvocationContext<
            TRequest,
            TResponse
        >);

        private readonly IInvocationComponentResolver<
            IInvocationMiddleware<TRequest, TResponse>
        > _resolver;

        /// <inheritdoc/>
        public Type MiddlewareType { get; private set; }

        /// <inheritdoc/>
        public int Order { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="DefaultMiddlewareInvocationPipelineItem{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="order"><inheritdoc cref="IMiddlewareInvocationPipelineItem.Order" path="/summary"/></param>
        /// <param name="middlewareType"><inheritdoc cref="IMiddlewareInvocationPipelineItem.MiddlewareType" path="/summary"/></param>
        /// <param name="resolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationMiddleware{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null" />.</exception>
        public DefaultMiddlewareInvocationPipelineItem(
            int order,
            Type middlewareType,
            IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> resolver
        )
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (middlewareType is null)
            {
                throw new ArgumentNullException(nameof(middlewareType));
            }

            Order = order;
            MiddlewareType = middlewareType;
            _resolver = resolver;
        }

        /// <inheritdoc/>
        public bool IsApplicable(Type contextType)
        {
            return s_matchingInvocationContextType.IsAssignableFrom(contextType);
        }

        /// <inheritdoc/>
        public Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        )
        {
            var middleware = _resolver.ResolveComponent();
            return middleware.HandleAsync(context, next, cancellationToken);
        }
    }
}
