using Vesture.Mediator.Commands;
using Vesture.Mediator.Events;
using Vesture.Mediator.Requests;

namespace Vesture.Mediator.Invocation
{
    /// <summary>
    /// An <see cref="IInvocationContext"/> defines an invocation context
    /// for a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.
    /// This context is used throughout the mediator pipeline to hold and manage the state of the request,
    /// response, and any errors that may occur during processing.
    /// </summary>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface IInvocationContext
    {
        /// <summary>
        /// Gets the type of <see cref="Request"/> being processed.
        /// </summary>
        Type RequestType { get; }

        /// <summary>
        /// Gets the actual <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>
        /// that needs to be handled in the current pipeline.
        /// </summary>
        object Request { get; }

        /// <summary>
        /// Indicates whether the <see cref="Request"/> corresponds to an <see cref="IEvent"/>.
        /// </summary>
        bool IsEvent { get; }

        /// <summary>
        /// Indicates whether the <see cref="Request"/> corresponds to a <see cref="ICommand"/>.
        /// </summary>
        bool IsCommand { get; }

        /// <summary>
        /// Indicates whether the <see cref="Request"/> corresponds to a <see cref="IRequest{TResponse}"/>.
        /// </summary>
        bool IsRequest { get; }

        /// <summary>
        /// Indicates whether a <see cref="ResponseType"/> is available for the current request.
        /// This is typically false for <see cref="ICommand"/> and <see cref="IEvent"/>.
        /// </summary>
        bool HasResponseType { get; }

        /// <summary>
        /// Gets the type of <see cref="Response"/> that is expected as a result of processing the <see cref="Request"/>.
        /// This is <c>null</c> if the <see cref="Request"/> is a <see cref="ICommand"/> or <see cref="IEvent"/>.
        /// </summary>
        Type? ResponseType { get; }

        /// <summary>
        /// Gets the actual response produced after processing the <see cref="Request"/>.
        /// This is <c>null</c> if the <see cref="Request"/> is a <see cref="ICommand"/> or <see cref="IEvent"/>.
        /// </summary>
        object? Response { get; }

        /// <summary>
        /// Indicates whether a response has been set after processing the <see cref="Request"/>.
        /// </summary>
        bool HasResponse { get; }

        /// <summary>
        /// Gets the <see cref="Exception"/> that may have occurred during the processing of the <see cref="Request"/>.
        /// </summary>
        Exception? Error { get; }

        /// <summary>
        /// Indicates whether an error has occurred during the processing of the <see cref="Request"/>.
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Indicates whether no error has occurred and the <see cref="Request"/> has been successfully processed.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Records the response that was produced after the processing of the <see cref="Request"/>.
        /// </summary>
        /// <param name="response">The response object to set.</param>
        void SetResponse(object? response);

        /// <summary>
        /// Records an <see cref="Exception"/> that occurred during the processing of the <see cref="Request"/>.
        /// </summary>
        /// <param name="error">The exception to set.</param>
        void SetError(Exception? error);

        /// <summary>
        /// Records an <see cref="Exception"/> that occurred during the processing of the <see cref="Request"/>
        /// and aggregates it with the other <see cref="Exception"/> recorded previously.
        /// </summary>
        /// <param name="error">The exception to add.</param>
        void AddError(Exception error);
    }

    /// <typeparam name="TResponse">The response type as expected by the <see cref="IInvocationContext.Request"/>.</typeparam>
    /// <inheritdoc cref="IInvocationContext"/>
    public interface IInvocationContext<out TResponse> : IInvocationContext
    {
        /// <inheritdoc cref="IInvocationContext.Response"/>
        new TResponse? Response { get; }
    }

    /// <exclude />
    /// <inheritdoc cref="IInvocationContext{TResponse}"/>
    /// <typeparam name="TRequest">The request type as specified by <see cref="Request"/>.</typeparam>
    /// <typeparam name="TResponse">The expected response type from the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</typeparam>
    public interface IInvocationContext<out TRequest, out TResponse> : IInvocationContext<TResponse>
    {
        /// <inheritdoc cref="IInvocationContext.Request"/>
        new TRequest Request { get; }
    }
}
