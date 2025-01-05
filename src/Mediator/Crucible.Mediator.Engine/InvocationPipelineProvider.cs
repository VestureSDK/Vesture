using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine
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
        /// <param name="pipelines">The <see cref="IServiceProvider"/> instance.</param>
        public InvocationPipelineProvider(IDictionary<(Type request, Type response), InvocationPipeline> pipelines)
        {
            Pipelines = pipelines;
        }

        /// <summary>
        /// The <see cref="IServiceProvider"/> instance.
        /// </summary>
        protected IDictionary<(Type request, Type response), InvocationPipeline> Pipelines { get; }

        /// <inheritdoc/>
        public virtual InvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request)
        {
            var requestType = request.GetType();
            if (Pipelines.TryGetValue((requestType, typeof(TResponse)), out var p) && p is InvocationPipeline<TResponse> pipeline)
            {
                return pipeline;
            }
            else
            {
                throw new KeyNotFoundException("Service not found");
            }
        }
    }
}
