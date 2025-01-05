using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Strategies
{
    /// <summary>
    /// Base implementation of <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/>
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class RequestHandlerStrategy<TRequest, TResponse> : IRequestHandlerStrategy<TRequest, TResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task ExecuteHandlerAsync(IInvocationHandler<TRequest, TResponse> handler, IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await handler.HandleAsync(context.Request, cancellationToken).ConfigureAwait(false);
                context.SetResponse(response);
            }
            catch (Exception ex)
            {
                context.SetError(ex);
            }
        }

        /// <inheritdoc/>
        public abstract Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default);
    }
}
