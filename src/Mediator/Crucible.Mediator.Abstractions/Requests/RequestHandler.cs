using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Base implementation of <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// </summary>
    /// <remarks>
    /// Override <see cref="InvocationHandler{TRequest, TResponse}.ExecuteAsync(TRequest, CancellationToken)"/> to handle the request as specified by <typeparamref name="TRequest"/>.
    /// </remarks>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    public abstract class RequestHandler<TRequest, TResponse> : InvocationHandler<TRequest, TResponse>, IRequestHandler<TRequest, TResponse>
    {

    }
}
