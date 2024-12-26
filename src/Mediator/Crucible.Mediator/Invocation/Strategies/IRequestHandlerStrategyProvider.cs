namespace Crucible.Mediator.Invocation.Strategies
{
    public interface IRequestHandlerStrategyProvider
    {
        IRequestHandlerStrategy<TRequest, TResponse> GetRequestHandlerStrategyForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context);
    }
}
