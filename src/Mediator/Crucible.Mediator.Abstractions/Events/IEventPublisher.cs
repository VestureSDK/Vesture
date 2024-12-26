using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Defines an <see cref="IEvent"/> publisher.
    /// </summary>
    /// <remarks>
    /// For simplicity, you should rather use <see cref="IMediator"/> directly.
    /// </remarks>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>The publication and handling process.</returns>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <paramref name="event"/> and returns the <see cref="IInvocationContext"/> 
        /// containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
