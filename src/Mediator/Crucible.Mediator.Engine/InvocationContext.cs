using System.Diagnostics;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine
{
    /// <summary>
    /// Default implementation of <see cref="IInvocationContext"/>.
    /// </summary>
    [DebuggerDisplay("{RequestType.Name} -> {ResponseType.Name}")]
    public abstract class InvocationContext : IInvocationContext
    {
        /// <summary>
        /// Initializes a new <see cref="InvocationContext"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> instance.</param>
        public InvocationContext(object request)
        {
            Request = request;
        }

        /// <inheritdoc/>
        public void SetResponse(object? response)
        {
            Response = response;
        }

        /// <inheritdoc/>
        public abstract Type RequestType { get; }

        /// <inheritdoc/>
        public object Request { get; }

        /// <inheritdoc/>
        public bool IsEvent { get; set; }

        /// <inheritdoc/>
        public bool HasResponseType => ResponseType.IsAssignableTo(NoResponse.Type);

        /// <inheritdoc/>
        public abstract Type ResponseType { get; }

        /// <inheritdoc/>
        public object? Response { get; private set; }

        /// <inheritdoc/>
        public bool HasResponse => Response != null;

        /// <inheritdoc/>
        public void SetError(Exception? error)
        {
            if (Error is null || error is null)
            {
                Error = error;
            }
            else
            {
                var errors = AggregateErrors(Error);
                Error = new AggregateException(errors);
            }
        }

        private static List<Exception> AggregateErrors(Exception existingError)
        {
            var errors = new List<Exception>();

            if (existingError is AggregateException aggregateException)
            {
                errors.AddRange(aggregateException.InnerExceptions);
            }
            else
            {
                errors.Add(existingError);
            }

            return errors;
        }

        /// <inheritdoc/>
        public Exception? Error { get; private set; }

        /// <inheritdoc/>
        public bool HasError => Error != null;

        /// <inheritdoc/>
        public bool IsSuccess => !HasError;
    }

    /// <summary>
    /// Default implementation of <see cref="IInvocationContext{TResponse}"/> and <see cref="IInvocationContext{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> type.</typeparam>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    [DebuggerDisplay("{typeof(TRequest).Name} -> {typeof(TResponse).Name}")]
    public class InvocationContext<TRequest, TResponse> : InvocationContext, IInvocationContext<TRequest, TResponse>, IInvocationContext<TResponse>
    {
        /// <inheritdoc/>
        public InvocationContext(TRequest request)
            : base(request!) { }

        /// <inheritdoc/>
        public override Type RequestType => typeof(TRequest);

        /// <inheritdoc/>
        public override Type ResponseType => typeof(TResponse);

        /// <inheritdoc/>
        TRequest IInvocationContext<TRequest, TResponse>.Request => (TRequest)Request;

        /// <inheritdoc/>
        TResponse? IInvocationContext<TResponse>.Response => (TResponse?)Response;
    }
}
