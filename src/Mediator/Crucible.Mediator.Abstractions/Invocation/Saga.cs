using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    public interface ISaga
    {
        IMediator Mediator { set; }
    }

    public abstract class Saga<TCommand> : CommandHandler<TCommand>, ISaga
        where TCommand : ICommand
    {
        public IMediator Mediator { private get; set; }

        protected Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => Mediator.ExecuteAndCaptureAsync(request, cancellationToken);

        protected Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => Mediator.ExecuteAsync(request, cancellationToken);

        protected Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default) => Mediator.HandleAndCaptureAsync<TResponse>(request, cancellationToken);

        protected Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default) => Mediator.HandleAsync<TResponse>(request, cancellationToken);

        protected Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default) => Mediator.InvokeAndCaptureAsync(command, cancellationToken);

        protected Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default) => Mediator.InvokeAsync(command, cancellationToken);

        protected Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default) => Mediator.PublishAndCaptureAsync(@event, cancellationToken);

        protected Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default) => Mediator.PublishAsync(@event, cancellationToken);
    }

    public abstract class Saga<TRequest, TResponse> : RequestHandler<TRequest, TResponse>, ISaga
        where TRequest : IRequest<TResponse>
    {
        public IMediator Mediator { private get; set; }

        protected Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => Mediator.ExecuteAndCaptureAsync(request, cancellationToken);

        protected Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => Mediator.ExecuteAsync(request, cancellationToken);

        protected Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default) => Mediator.HandleAndCaptureAsync<TResponse>(request, cancellationToken);

        protected Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default) => Mediator.HandleAsync<TResponse>(request, cancellationToken);

        protected Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default) => Mediator.InvokeAndCaptureAsync(command, cancellationToken);

        protected Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default) => Mediator.InvokeAsync(command, cancellationToken);

        protected Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default) => Mediator.PublishAndCaptureAsync(@event, cancellationToken);

        protected Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default) => Mediator.PublishAsync(@event, cancellationToken);
    }
}
