using System.Runtime.CompilerServices;
using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation.Strategies
{
    public class SingleHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> _resolver;

        public SingleHandlerStrategy(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver)
        {
            _resolver = resolver;
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            return InvokeHandlerAsync(_resolver, context, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static async Task InvokeHandlerAsync(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver, IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            var handler = resolver.ResolveComponent();
            var response = await handler.HandleAsync(context.Request, cancellationToken);
            context.SetResponse(response);
        }
    }
}
