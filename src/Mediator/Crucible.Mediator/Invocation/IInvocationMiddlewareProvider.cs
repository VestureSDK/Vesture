﻿using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="context"></param>
        /// <returns></returns>
        IReadOnlyList<InvocationMiddlewareWrapper> GetMiddlewaresForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context);
    }

    /// <summary>
    /// Default implementation of <see cref="IInvocationMiddlewareProvider"/>.
    /// </summary>
    public class InvocationMiddlewareProvider : IInvocationMiddlewareProvider
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new <see cref="InvocationMiddlewareProvider"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/> instance.</param>
        public InvocationMiddlewareProvider(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc/>
        public IReadOnlyList<InvocationMiddlewareWrapper> GetMiddlewaresForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context)
        {
            // Resolve the middlewares with their defined lifetime and
            // order them by their defined Order. Then reverse the collection
            // to ensure the outer most handler is executed first.
            var contextType = context.GetType();
            var middlewares = _services.GetServices<InvocationMiddlewareWrapper>().ToList();
            for (int i = middlewares.Count -1; i>=0; i--)
            {
                if (!middlewares[i].IsApplicable(context, contextType))
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
