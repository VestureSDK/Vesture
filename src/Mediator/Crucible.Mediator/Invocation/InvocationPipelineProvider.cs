using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Default implementation of <see cref="IInvocationPipelineProvider"/>.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="IServiceProvider"/> under the hood to resolve the <see cref="InvocationPipeline{TResponse}"/> for
    /// <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.
    /// </remarks>
    public class InvocationPipelineProvider : IInvocationPipelineProvider
    {
        /// <summary>
        /// Initializes a new <see cref="InvocationPipelineProvider"/> instance.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance.</param>
        public InvocationPipelineProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// The <see cref="IServiceProvider"/> instance.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public virtual InvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request)
        {
            return ServiceProvider.GetRequiredKeyedService<InvocationPipeline<TResponse>>(request.GetType());
        }
    }
}
