using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation.Strategies
{
    /// <summary>
    /// An <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> is responsible to manage how 
    /// <see cref="IInvocationHandler{TRequest, TResponse}"/> are invoked as part of a <see cref="IInvocationPipeline{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The contract type handled by the <see cref="IInvocationHandler{TRequest, TResponse}"/> to invoke.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The response type produced by processing the <typeparamref name="TRequest"/> the <see cref="IInvocationHandler{TRequest, TResponse}"/> to invoke.
    /// </typeparam>
    public interface IInvocationHandlerStrategy<TRequest, TResponse> : IInvocationMiddleware<TRequest, TResponse>
    {

    }
}
