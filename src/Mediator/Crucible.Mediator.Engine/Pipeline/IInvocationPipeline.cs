using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    public interface IInvocationPipeline
    {
        Type Request { get; }

        Type Response { get; }
    }

    public interface IInvocationPipeline<TResponse> : IInvocationPipeline
    {
        Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken);
    }
}
