using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines a base <see cref="InvocationMiddlewareWrapper"/> for registration in the <see cref="IServiceCollection"/>
    /// </summary>
    public abstract class InvocationMiddlewareWrapper
    {
        public int Order { get; }

        protected InvocationMiddlewareWrapper(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Defines if the underlying <see cref="IInvocationMiddleware{TRequest, TResponse}"/> 
        /// is applicable for the <paramref name="contextType"/>.
        /// </summary>
        /// <param name="contextType">The <see cref="Type"/> of <see cref="IInvocationContext{TRequest, TResponse}"/>.</param>
        /// <returns><c>True</c> if the underlying <see cref="IInvocationMiddleware{TRequest, TResponse}"/> is applicable for <paramref name="context"/>; otherwise <c>False</c>.</returns>
        public abstract bool IsApplicable(Type contextType);

        /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}.ExecuteAsync(IInvocationContext{TRequest, TResponse}, Func{CancellationToken, ValueTask}, CancellationToken)"/>
        public abstract Task ExecuteAsync(IInvocationContext context, Func<CancellationToken, Task> next, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Defines a wrapper around a <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for registration and resolution with <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> request type.</typeparam>
    /// <typeparam name="TResponse">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> response type.</typeparam>
    public class InvocationMiddlewareWrapper<TRequest, TResponse> : InvocationMiddlewareWrapper
    {
        private readonly static Type _matchingInvocationContextType = typeof(IInvocationContext<TRequest, TResponse>);

        private readonly Lazy<IInvocationMiddleware<TRequest, TResponse>> _middlewareInitializer;

        private IInvocationMiddleware<TRequest, TResponse> _middleware => _middlewareInitializer.Value;

        /// <summary>
        /// Initializes a new <see cref="InvocationMiddlewareWrapper{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="middlewareInitializer">The <see cref="Lazy{T}"/> of the <see cref="IInvocationMiddleware{TRequest, TResponse}"/>.</param>
        public InvocationMiddlewareWrapper(int order, Lazy<IInvocationMiddleware<TRequest, TResponse>> middlewareInitializer)
            : base(order)
        {
            _middlewareInitializer = middlewareInitializer;
        }

        /// <inheritdoc/>
        public override bool IsApplicable(Type contextType)
        {
            return _matchingInvocationContextType.IsAssignableFrom(contextType);
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync(IInvocationContext context, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
        {
            return _middleware.ExecuteAsync((IInvocationContext<TRequest, TResponse>)context, next, cancellationToken);
        }
    }
}
