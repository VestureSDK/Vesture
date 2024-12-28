using System.Diagnostics;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation.Strategies;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines an invocation pipeline to execute a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.
    /// </summary>
    /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</typeparam>
    [DebuggerDisplay("??? -> {typeof(TResponse).Name}")]
    public abstract class InvocationPipeline<TResponse>
    {
        /// <summary>
        /// Executes the specified <paramref name="request"/> and returns the <see cref="IInvocationContext{TResponse}"/> containing
        /// the expected <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> or <see cref="ICommand"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </returns>
        public abstract Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync(object request, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc cref="InvocationPipeline{TResponse}"/>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> type.</typeparam>
    /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</typeparam>
    [DebuggerDisplay("{typeof(TRequest).Name} -> {typeof(TResponse).Name}")]
    public class InvocationPipeline<TRequest, TResponse> : InvocationPipeline<TResponse>
    {
        private readonly IInvocationMiddlewareProvider _middlewareProvider;

        private readonly IRequestHandlerStrategy<TRequest, TResponse> _strategy;

        private readonly IInvocationContextFactory _contextFactory;

        /// <summary>
        /// Initializes a new <see cref="InvocationPipeline{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="contextFactory">The <see cref="IInvocationContextFactory"/> instances.</param>
        /// <param name="middlewareProvider">The <see cref="IInvocationMiddlewareProvider"/> instances.</param>
        /// <param name="strategy">The <see cref="IRequestHandlerStrategyProvider"/> instances.</param>
        public InvocationPipeline(
            IInvocationContextFactory contextFactory,
            IInvocationMiddlewareProvider middlewareProvider,
            IRequestHandlerStrategy<TRequest, TResponse> strategy)
        {
            _contextFactory = contextFactory;
            _middlewareProvider = middlewareProvider;
            _strategy = strategy;
        }

        /// <inheritdoc/>
        public override async Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync(object request, CancellationToken cancellationToken = default)
        {
            // Creates the context the pipeline will process
            var context = _contextFactory.CreateContextForRequest<TRequest, TResponse>(request);

            // Creates a chain of responsibility
            var chainOfResponsibility = CreateChainOfresponsibility(context);

            // Invoke the chain of responsibility and returns the context
            // affected by all the middlewares and the handler
            await chainOfResponsibility.Invoke(cancellationToken);

            return context;
        }

        private Func<CancellationToken, ValueTask> CreateChainOfresponsibility(IInvocationContext<TRequest, TResponse> context)
        {
            // Creates a chain of responsibility with
            // all the middlewares and finish by the handler
            Func<CancellationToken, ValueTask> root = (ct) =>
            {
                var task = _strategy.ExecuteAsync(context, ct);
                return new ValueTask(task);
            };

            // Build the chain of responsibility and return the new root func.
            var middlewares = _middlewareProvider.GetMiddlewaresForContext(context);
            for (int i = middlewares.Count - 1; i >= 0; i--)
            {
                var prevNext = root;
                var middleware = middlewares[i];
                root = (ct) => middleware.ExecuteAsync(context, prevNext, ct);
            }

            return root;
        }
    }
}
