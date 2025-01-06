using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    public interface IInvocationPipelineItem<TRequest, TResponse>
    {
        Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> next, CancellationToken cancellationToken);
    }
}
