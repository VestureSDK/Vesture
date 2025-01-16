namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    public interface INoOpInvocationPipelineProvider
    {
        IInvocationPipeline<TResponse> GetNoOpInvocationPipeline<TResponse>();
    }
}