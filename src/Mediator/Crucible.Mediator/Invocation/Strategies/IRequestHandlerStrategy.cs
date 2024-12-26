using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    public interface IRequestHandlerStrategy<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {

    }
}
