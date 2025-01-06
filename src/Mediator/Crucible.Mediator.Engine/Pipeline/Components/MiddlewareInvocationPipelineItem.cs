using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Components
{
    public class MiddlewareInvocationPipelineItem<TRequest, TResponse> : IMiddlewareInvocationPipelineItem, IInvocationPipelineItem<TRequest, TResponse>
    {
        private readonly static Type _matchingInvocationContextType = typeof(IInvocationContext<TRequest, TResponse>);

        private readonly IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> _resolver;

        public int Order { get; private set; }

        public MiddlewareInvocationPipelineItem(int order, IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> resolver)
        {
            Order = order;
            _resolver = resolver;
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            var middleware = _resolver.ResolveComponent();
            return middleware.HandleAsync(
                context,
                (ct) => next.Invoke(context, ct),
                cancellationToken);
        }

        public bool IsApplicable(Type contextType)
        {
            return _matchingInvocationContextType.IsAssignableFrom(contextType);
        }
    }
}
