using System.Runtime.CompilerServices;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Requests;

namespace Ingot.Mediator.Engine.Pipeline.Strategies
{
    /// <summary>
    /// <para>
    /// The <see cref="SingleHandlerStrategy{TRequest, TResponse}"/> is a <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// invoking the <see cref="IInvocationHandler{TRequest, TResponse}"/> and awaiting its execution.
    /// </para>
    /// <para>
    /// It is the default <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> for 
    /// <see cref="IRequest{TResponse}"/> and <see cref="ICommand"/>.
    /// </para>
    /// </summary>
    /// <inheritdoc cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>
    public class SingleHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> _resolver;

        /// <summary>
        /// Initializes a new <see cref="SingleHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="resolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationHandler{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null" />.</exception>
        public SingleHandlerStrategy(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver)
        {
            ArgumentNullException.ThrowIfNull(resolver, nameof(resolver));

            _resolver = resolver;
        }

        /// <inheritdoc/>
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
