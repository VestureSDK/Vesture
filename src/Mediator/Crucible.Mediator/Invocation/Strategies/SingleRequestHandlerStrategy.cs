using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> executing the underlying <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class SingleRequestHandlerStrategy<TRequest, TResponse> : RequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;

        /// <summary>
        /// Initializes a new <see cref="SingleRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handler">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instance.</param>
        public SingleRequestHandlerStrategy(
            IRequestHandler<TRequest, TResponse> handler)
        {
            _handler = handler;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            return ExecuteHandlerAsync(_handler, context, cancellationToken);
        }
    }
}
