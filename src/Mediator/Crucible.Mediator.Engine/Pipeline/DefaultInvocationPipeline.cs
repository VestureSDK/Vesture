using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    /// <summary>
    /// <para>
    /// The <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> provides a default implementation of <see cref="IInvocationPipeline{TResponse}"/>.
    /// </para>
    /// <para>
    /// An <see cref="IInvocationPipeline{TResponse}"/> represents the orchestrated sequence of execution 
    /// that processes a specific contract through a series of <see cref="IInvocationMiddleware{TRequest, TResponse}"/> 
    /// and ultimately reaches an <see cref="IInvocationHandler{TRequest, TResponse}"/>.
    /// </para> 
    /// </summary>
    /// <typeparam name="TRequest">The type of contract handled by this pipeline.</typeparam>
    /// <typeparam name="TResponse"><inheritdoc cref="IInvocationPipeline{TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
    /// <inheritdoc cref="IInvocationPipeline{TResponse}"/>
    public class DefaultInvocationPipeline<TRequest, TResponse> : IInvocationPipeline<TResponse>
    {
        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly IInvocationHandlerStrategy<TRequest, TResponse> _handler;

        /// <inheritdoc />
        public Type RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        public Type ResponseType { get; } = typeof(TResponse);

        /// <summary>
        /// Initializes a new <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="contextFactory">The <see cref="IInvocationContextFactory"/> instance.</param>
        /// <param name="preInvocationPipelineMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IPrePipelineMiddleware"/> instance.</param>
        /// <param name="middlewares">The <see cref="IMiddlewareInvocationPipelineItem"/> instances.</param>
        /// <param name="preHandlerMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IPreHandlerMiddleware"/> instance.</param>
        /// <param name="handlerStrategy">The <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="contextFactory"/> is <see langword="null" />.</item>
        /// <item><paramref name="preInvocationPipelineMiddlewareResolver"/> is <see langword="null" />.</item>
        /// <item><paramref name="middlewares"/> is <see langword="null" />.</item>
        /// <item><paramref name="preHandlerMiddlewareResolver"/> is <see langword="null" />.</item>
        /// <item><paramref name="handlerStrategy"/> is <see langword="null" />.</item>
        /// </list>
        /// </exception>
        public DefaultInvocationPipeline(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handlerStrategy)
        {
            ArgumentNullException.ThrowIfNull(contextFactory, nameof(contextFactory));
            ArgumentNullException.ThrowIfNull(preInvocationPipelineMiddlewareResolver, nameof(preInvocationPipelineMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(middlewares, nameof(middlewares));
            ArgumentNullException.ThrowIfNull(preHandlerMiddlewareResolver, nameof(preHandlerMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(handlerStrategy, nameof(handlerStrategy));

            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handler = handlerStrategy;
            _chainOfResponsibility = new Lazy<Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task>>(CreateChainOfresponsibility);
        }

        private readonly Lazy<Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task>> _chainOfResponsibility;

        private Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> CreateChainOfresponsibility()
        {
            var middlewares = new List<IInvocationMiddleware<TRequest, TResponse>>();

            var contextType = typeof(IInvocationContext<TRequest, TResponse>);
            foreach (var middleware in _middlewares.OrderBy(m => m.Order))
            {
                if (middleware.IsApplicable(contextType))
                {
                    middlewares.Add((IInvocationMiddleware<TRequest, TResponse>)middleware);
                }
            }

            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> chain = (ctx, ct) =>
            {
                var preHandlerMiddleware = _preHandlerMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync((IInvocationContext<object, object>)ctx, (t) => handler(ctx, t), ct);
            };

            // Build the chain of responsibility and return the new root func.
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                var nextMiddleware = chain;
                var item = middlewares[i];
                chain = (ctx, ct) => item.HandleAsync(ctx, (t) => nextMiddleware.Invoke(ctx, t), ct);
            }

            var next = chain;
            chain = (ctx, ct) =>
            {
                var preHandlerMiddleware = _preInvocationPipelineMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync((IInvocationContext<object, object>)ctx, (t) => next.Invoke(ctx, t), ct);
            };

            return chain!;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Task handler(IInvocationContext<TRequest, TResponse> ctx, CancellationToken ct) => _handler.HandleAsync(ctx, null, ct);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        /// <inheritdoc />
        public async Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken)
        {
            var context = _contextFactory.CreateContextForRequest<TRequest, TResponse>(request);
            await _chainOfResponsibility.Value.Invoke(context, cancellationToken);
            return context;
        }
    }
}
