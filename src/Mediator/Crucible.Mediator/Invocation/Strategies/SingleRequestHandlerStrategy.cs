using Crucible.Mediator.Invocation.Accessors;
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
        private readonly IInvocationComponentAccessor<IRequestHandler<TRequest, TResponse>> _handlerAccessor;

        /// <summary>
        /// Initializes a new <see cref="SingleRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handler">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instance.</param>
        public SingleRequestHandlerStrategy(IInvocationComponentAccessor<IRequestHandler<TRequest, TResponse>> handlerAccessor)
        {
            _handlerAccessor = handlerAccessor;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            var handler = _handlerAccessor.GetComponent();
            return ExecuteHandlerAsync(handler, context, cancellationToken);
        }
    }
}
