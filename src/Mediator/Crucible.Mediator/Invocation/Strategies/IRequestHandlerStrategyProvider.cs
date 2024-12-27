namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// Defines a <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> provider.
    /// </summary>
    public interface IRequestHandlerStrategyProvider
    {
        /// <summary>
        /// Gets the <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> for the specified <paramref name="context"/>.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context">The <see cref="IInvocationContext{TRequest, TResponse}"/> to get the strategy for.</param>
        /// <returns>The relevant <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/>.</returns>
        IRequestHandlerStrategy<TRequest, TResponse> GetRequestHandlerStrategyForContext<TRequest, TResponse>(IInvocationContext<TRequest, TResponse> context);
    }
}
