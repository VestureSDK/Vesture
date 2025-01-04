namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> provider.
    /// </summary>
    public interface IInvocationMiddlewareProvider
    {
        /// <summary>
        /// Gets the <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for the specified <paramref name="context"/>.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        IReadOnlyList<InvocationMiddlewareWrapper> GetMiddlewaresForContext<TRequest, TResponse>();
    }

    /// <summary>
    /// Default implementation of <see cref="IInvocationMiddlewareProvider"/>.
    /// </summary>
    public class InvocationMiddlewareProvider : IInvocationMiddlewareProvider
    {
        private readonly IEnumerable<InvocationMiddlewareWrapper> _middlewareWrappers;

        /// <summary>
        /// Initializes a new <see cref="InvocationMiddlewareProvider"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/> instance.</param>
        public InvocationMiddlewareProvider(IEnumerable<InvocationMiddlewareWrapper> middlewareWrappers)
        {
            _middlewareWrappers = middlewareWrappers;
        }

        /// <inheritdoc/>
        public IReadOnlyList<InvocationMiddlewareWrapper> GetMiddlewaresForContext<TRequest, TResponse>()
        {
            // Resolve the middlewares with their defined lifetime and
            // order them by their defined Order. Then reverse the collection
            // to ensure the outer most handler is executed first.
            var contextType = typeof(IInvocationContext<TRequest, TResponse>);
            var middlewares = _middlewareWrappers.ToList();
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                if (!middlewares[i].IsApplicable(contextType))
                {
                    middlewares.RemoveAt(i);
                }
            }

            if (middlewares.Count > 1)
            {
                middlewares.Sort((a, b) => a.Order.CompareTo(b.Order));
            }

            return middlewares;
        }
    }
}
