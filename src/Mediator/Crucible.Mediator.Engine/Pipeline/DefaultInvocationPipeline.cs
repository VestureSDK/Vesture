using Crucible.Mediator.Engine.Pipeline.Components;
using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Invocation;
using Crucible.Mediator.Engine.Pipeline.Invocation.Strategies;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    public class DefaultInvocationPipeline<TRequest, TResponse> : IInvocationPipeline<TResponse>
    {
        private readonly IInvocationContextFactory _contextFactory;
        private readonly IInvocationComponentResolver<IPreInvocationPipelineMiddleware> _preInvocationPipelineMiddlewareResolver;
        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;
        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;
        private readonly IInvocationHandlerStrategy<TRequest, TResponse> _handler;

        public Type Request { get; } = typeof(TRequest);

        public Type Response { get; } = typeof(TResponse);

        public DefaultInvocationPipeline(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPreInvocationPipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handler)
        {
            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handler = handler;
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

        public async Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken)
        {
            var context = _contextFactory.CreateContextForRequest<TRequest, TResponse>((TRequest)request);
            await _chainOfResponsibility.Value.Invoke(context, cancellationToken);
            return context;
        }
    }
}
