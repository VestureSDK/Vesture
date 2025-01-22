using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// A <see cref="IRequestHandler{TRequest, TResponse}"/> is responsible for the actual
    /// logic of processing a specific <see cref="IRequest{TResponse}"/> contract.
    /// </para>
    /// <para>
    /// When an <see cref="IRequest{TResponse}"/> contract is sent to the mediator, the mediator
    /// routes it to the appropriate <see cref="IRequestHandler{TRequest, TResponse}"/>, which then
    /// processes the request and returns a <typeparamref name="TResponse"/>.
    /// It helps decouple request processing logic from the core application logic, enabling
    /// cleaner, more modular code.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">
    /// The <see cref="IRequest{TResponse}"/> contract type handled by this handler.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The response type produced by processing the <typeparamref name="TRequest"/>.
    /// </typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface IRequestHandler<TRequest, TResponse> : IInvocationHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> { }
}
