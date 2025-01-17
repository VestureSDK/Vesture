using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.Engine.Pipeline.Context
{
    /// <summary>
    /// The <see cref="DefaultInvocationContextFactory"/> provides 
    /// a default implementation of <see cref="IInvocationContextFactory"/> creating
    /// a <see cref="DefaultInvocationContext{TRequest, TResponse}"/>.
    /// </summary>
    public class DefaultInvocationContextFactory : IInvocationContextFactory
    {
        /// <inheritdoc/>
        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request)
        {
            var typedRequest = (TRequest)request;
            return new DefaultInvocationContext<TRequest, TResponse>(typedRequest);
        }
    }
}
