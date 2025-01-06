namespace Crucible.Mediator.Engine.Pipeline.Components
{
    public interface IMiddlewareInvocationPipelineItem
    {
        int Order { get; }

        bool IsApplicable(Type contextType);
    }
}
