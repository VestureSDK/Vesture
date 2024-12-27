using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines an invocation context for a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.
    /// </summary>
    public interface IInvocationContext
    {
        /// <summary>
        /// The type of <see cref="Request"/>.
        /// </summary>
        Type RequestType { get; }

        /// <summary>
        /// The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> that needs to be handled.
        /// </summary>
        object Request { get; }

        /// <summary>
        /// Defines if the <see cref="Request"/> is an <see cref="IEvent"/>.
        /// </summary>
        bool IsEvent { get; }

        /// <summary>
        /// Defines if a <see cref="ResponseType"/> is available.
        /// </summary>
        bool HasResponseType { get; }

        /// <summary>
        /// The type of <see cref="Response"/>.
        /// </summary>
        /// <remarks>
        /// <c>null</c> if the <see cref="Request"/> is a <see cref="ICommand"/> or <see cref="IEvent"/>.
        /// </remarks>
        Type? ResponseType { get; }

        /// <summary>
        /// The response as expected by <see cref="Request"/>.
        /// </summary>
        /// <remarks>
        /// <c>null</c> if the <see cref="Request"/> is a <see cref="ICommand"/> or <see cref="IEvent"/>.
        /// </remarks>
        object? Response { get; }

        /// <summary>
        /// Defines if a <see cref="Response"/> is available.
        /// </summary>
        bool HasResponse { get; }

        /// <summary>
        /// The <see cref="Exception"/> that has occured during processing.
        /// </summary>
        Exception? Error { get; }

        /// <summary>
        /// Defines if an <see cref="Error"/> has occured.
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Defines if no <see cref="Error"/> has occured.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Record the <see cref="Response"/> as expected by <see cref="Request"/> after its processing.
        /// </summary>
        /// <param name="response">The response to set.</param>
        void SetResponse(object? response);

        /// <summary>
        /// Records an <see cref="Exception"/> that occured during processing the <see cref="Request" />.
        /// </summary>
        /// <param name="error">The <see cref="Exception"/> to set.</param>
        void SetError(Exception? error);
    }

    /// <inheritdoc cref="IInvocationContext"/>
    /// <typeparam name="TResponse">The response as expected by <see cref="IInvocationContext.Request"/>.</typeparam>
    public interface IInvocationContext<out TResponse> : IInvocationContext
    {
        /// <inheritdoc/>
        new TResponse? Response { get; }
    }

    /// <inheritdoc cref="IInvocationContext{TResponse}"/>
    /// <typeparam name="TRequest">The request type as specified by <see cref="Request"/>.</typeparam>
    /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</typeparam>
    public interface IInvocationContext<out TRequest, out TResponse> : IInvocationContext<TResponse>
    {
        /// <inheritdoc/>
        new TRequest Request { get; }
    }
}
