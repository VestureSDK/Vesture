using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Components
{
    public class MiddlewareInvocationPipelineItem<TRequest, TResponse> : IMiddlewareInvocationPipelineItem, IInvocationMiddleware<TRequest, TResponse>
    {
        private readonly static Type _matchingInvocationContextType = typeof(IInvocationContext<TRequest, TResponse>);

        private readonly IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> _resolver;

        public int Order { get; private set; }

        public MiddlewareInvocationPipelineItem(int order, IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> resolver)
        {
            Order = order;
            _resolver = resolver;
        }

        public bool IsApplicable(Type contextType)
        {
            return _matchingInvocationContextType.IsAssignableFrom(contextType);
        }

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
