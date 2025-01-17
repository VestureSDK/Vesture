using Crucible.Mediator.Engine.Pipeline.Strategies;

namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    /// <summary>
    /// Defines an <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> resolver
    /// when no handlers have been registered for a specific contract.
    /// </summary>
    public interface INoOpInvocationHandlerStrategyResolver
    {
        /// <summary>
        /// Resolves the NoOp <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/>.
        /// </summary>
        /// <typeparam name="TResponse">The response contract.</typeparam>
        /// <returns>The resolved NoOp <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> instance.</returns>
        IInvocationHandlerStrategy<object, TResponse> ResolveNoOpInvocationHandlerStrategy<TResponse>();
    }
}
