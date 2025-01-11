using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
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
    /// <remarks>
    /// <para>
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> should not directly depend on or invoke 
    /// <see cref="IMediator"/> for subsequent operations, as this can lead to tightly 
    /// coupled and difficult-to-maintain code.
    /// </para>
    /// <para>
    /// Instead, a <seealso cref="RequestWorkflow{TRequest, TResponse}"/> should be used to orchestrate 
    /// the flow of operations. <see cref="IInvocationWorkflow"/> provide a higher-level abstraction for 
    /// managing complex workflows, ensuring that different handlers are executed in the 
    /// correct order while maintaining a clear separation of concerns. 
    /// </para>
    /// <para>
    /// By using workflows, you ensure that each handler remains focused on its individual 
    /// responsibility, leaving orchestration and sequencing to the workflows, thus adhering 
    /// to the principles of loose coupling and maintainability.
    /// </para>
    /// </remarks>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="CommandHandler{TRequest, TResponse}"/>
    /// <seealso cref="RequestWorkflow{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface IRequestHandler<TRequest, TResponse> : IInvocationHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

    }
}
