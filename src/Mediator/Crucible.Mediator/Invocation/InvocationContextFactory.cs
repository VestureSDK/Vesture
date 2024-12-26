using Crucible.Mediator.Events;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Default implementation of <see cref="IInvocationContextFactory"/>.
    /// </summary>
    public class InvocationContextFactory : IInvocationContextFactory
    {
        /// <inheritdoc/>
        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request)
        {
            var typedRequest = (TRequest)request;
            return new InvocationContext<TRequest, TResponse>(typedRequest)
            {
                IsEvent = typeof(TResponse) == EventResponse.Type,
            };
        }
    }
}
