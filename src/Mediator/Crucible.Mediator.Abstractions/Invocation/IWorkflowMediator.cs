using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <inheritdoc cref="IMediator"/>
    public interface IWorkflowMediator
    {
        /// <inheritdoc cref="IMediator.HandleAsync{TResponse}(Object, CancellationToken)"/>
        Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object contract, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IMediator.ExecuteAndCaptureAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IMediator.InvokeAndCaptureAsync(ICommand, CancellationToken)"/>
        Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IMediator.PublishAndCaptureAsync(IEvent, CancellationToken)"/>
        Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
