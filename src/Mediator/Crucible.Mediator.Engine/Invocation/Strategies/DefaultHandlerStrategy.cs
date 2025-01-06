using System.Runtime.CompilerServices;
using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Invocation.Strategies
{
    public class DefaultHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> _resolver;

        public DefaultHandlerStrategy(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver)
        {
            _resolver = resolver;
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            return InvokeHandlerAsync(_resolver, context, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static async Task InvokeHandlerAsync(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver, IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            var handler = resolver.ResolveComponent();
            var response = await handler.HandleAsync(context.Request, cancellationToken);
            context.SetResponse(response);
        }
    }
}
