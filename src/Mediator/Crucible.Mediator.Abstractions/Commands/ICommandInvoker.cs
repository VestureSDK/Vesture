using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines an invoker for <see cref="ICommand"/>.
    /// </summary>
    /// <remarks>
    /// For simplicity, you should rather use <see cref="IMediator"/> directly.
    /// </remarks>
    public interface ICommandInvoker
    {
        /// <summary>
        /// Executes the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified <paramref name="command"/> and returns the 
        /// <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
