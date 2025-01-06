using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Context
{
    /// <summary>
    /// Default implementation of <see cref="IInvocationContextFactory"/>.
    /// </summary>
    public class DefaultInvocationContextFactory : IInvocationContextFactory
    {
        /// <inheritdoc/>
        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request)
        {
            var typedRequest = (TRequest)request;
            return new DefaultInvocationContext<TRequest, TResponse>(typedRequest)
            {
                IsEvent = typeof(TResponse) == EventResponse.Type,
            };
        }
    }
}
