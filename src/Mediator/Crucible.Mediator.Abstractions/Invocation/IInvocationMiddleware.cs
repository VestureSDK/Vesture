using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// An <see cref="IInvocationMiddleware{TRequest, TResponse}"/> in the context of the mediator pattern provides 
    /// a mechanism for injecting cross-cutting concerns into the request, command, or event processing pipeline. 
    /// This interface allows you to define custom logic that  can be executed before, after, or even instead 
    /// of the actual handler for the given contract.
    /// </para>
    /// <para>
    /// <see cref="IInvocationMiddleware{TRequest, TResponse}"/> are part of a chain of responsibility pattern, 
    /// where each middleware has the opportunity to either pass control to the next middleware in the chain 
    /// or stop the process early by returning a response directly, effectively "shortcircuiting" the execution 
    /// of subsequent handlers. This flexibility makes it ideal for handling concerns such as logging, 
    /// validation, authorization, caching, and more.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of the request, command, or event being processed by the middleware.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response produced by the handler after processing the request, command, or event.
    /// </typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface IInvocationMiddleware<in TRequest, in TResponse>
    {
        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// The invocation context containing the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> contract,
        /// as well as the response or any <see cref="Exception"/> that may have occurred in the middleware/handler chain.
        /// </param>
        /// <param name="next">
        /// The delegate representing the next middleware or handler in the chain.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that propagates notification that the execution of the middleware and handler chain should be canceled.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous execution of the middleware and handler chain process.
        /// </returns>
        Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken);
    }
}
