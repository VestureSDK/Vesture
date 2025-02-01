using System.Runtime.CompilerServices;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Pipeline.Extensions;
using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Requests;
using Microsoft.Extensions.Logging;

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
    public class SingleHandlerStrategy<TRequest, TResponse>
        : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        private readonly IInvocationComponentResolver<
            IInvocationHandler<TRequest, TResponse>
        > _resolver;

        /// <summary>
        /// Initializes a new <see cref="SingleHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
        /// <param name="resolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IInvocationHandler{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" /> or <paramref name="resolver"/> is <see langword="null" />.</exception>
        public SingleHandlerStrategy(
            ILogger<SingleHandlerStrategy<TRequest, TResponse>> logger,
            IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver
        )
        {
            if (logger is null) { throw new ArgumentNullException(nameof(logger)); }
            if (resolver is null) { throw new ArgumentNullException(nameof(resolver)); }

            _logger = logger;
            _resolver = resolver;
        }

        /// <inheritdoc/>
        public Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        )
        {
            return InvokeHandlerAsync(_logger, _resolver, context, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static async Task InvokeHandlerAsync(
            ILogger logger,
            IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver,
            IInvocationContext<TRequest, TResponse> context,
            CancellationToken cancellationToken
        )
        {
            using var activity =
                MediatorEngineDiagnostics.s_invocationHandlerActivitySource.StartActivity(
                    "Handler Invocation"
                );

            // Set the activity status as error since it will be switched
            // back to "OK" if no errors are thrown.
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);

            var handler = resolver.ResolveComponent();

            logger.InvokingHandler(handler);

            var response = await handler.HandleAsync(context.Request, cancellationToken);
            context.SetResponse(response);

            logger.InvokedHandler(handler);

            // Set the activity status as "OK" since no error has been thrown
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
        }
    }
}
