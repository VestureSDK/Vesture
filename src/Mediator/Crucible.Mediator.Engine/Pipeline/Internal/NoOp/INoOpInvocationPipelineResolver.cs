namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    /// <summary>
    /// Defines an <see cref="IInvocationPipeline{TResponse}"/> resolver
    /// when no handlers have been registered for a specific contract.
    /// </summary>
    public interface INoOpInvocationPipelineResolver
    {
        /// <summary>
        /// Resolves the NoOp <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TResponse">The response contract.</typeparam>
        /// <returns>The resolved NoOp <see cref="IInvocationPipeline{TResponse}"/> instance.</returns>
        IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>();
    }
}
