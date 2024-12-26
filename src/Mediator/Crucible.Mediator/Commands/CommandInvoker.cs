using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Default implementation of <see cref="ICommandInvoker"/>.
    /// </summary>
    /// <remarks>
    /// It uses <see cref="IInvocationPipelineProvider"/> under the hood to resolve an <see cref="InvocationPipeline{TResponse}"/>
    /// and execute the middleware and handler related to a <see cref="ICommand"/>.
    /// </remarks>
    public class CommandInvoker : Invoker, ICommandInvoker
    {
        /// <summary>
        /// Initializes a new <see cref="CommandInvoker"/> instance.
        /// </summary>
        /// <param name="pipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        public CommandInvoker(IInvocationPipelineProvider pipelineProvider)
            : base(pipelineProvider) { }

        /// <inheritdoc/>
        public async Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAndCaptureAsync<CommandResponse>(command, cancellationToken);
        }

        /// <inheritdoc/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return HandleAsync<CommandResponse>(command, cancellationToken);
        }
    }
}
