using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// Defines a <see cref="IRequestHandler{TRequest, TResponse}"/> execution strategy.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestHandlerStrategy<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {

    }
}
