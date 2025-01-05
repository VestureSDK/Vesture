using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    public interface IInvocationPipelineItem<TRequest, TResponse>
    {
        Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<IInvocationContext<TRequest, TResponse>, Task> next, CancellationToken cancellationToken);
    }
}
