namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    public interface INoOpInvocationPipelineResolver
    {
        IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>();
    }
}
