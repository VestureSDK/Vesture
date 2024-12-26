using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Default implementation of <see cref="IRequestExecutor"/>.
    /// </summary>
    /// <remarks>
    /// It uses <see cref="IInvocationPipelineProvider"/> under the hood to resolve an <see cref="InvocationPipeline{TResponse}"/>
    /// and execute the middlewares and handler related to a <see cref="IRequest{TResponse}"/>.
    /// </remarks>
    public class RequestInvoker : Invoker, IRequestExecutor
    {
        /// <summary>
        /// Initializes a new <see cref="RequestInvoker"/> instance.
        /// </summary>
        /// <param name="pipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        public RequestInvoker(IInvocationPipelineProvider pipelineProvider)
            : base(pipelineProvider) { }

        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAndCaptureAsync<TResponse>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAsync<TResponse>(request, cancellationToken);
        }
    }
}
