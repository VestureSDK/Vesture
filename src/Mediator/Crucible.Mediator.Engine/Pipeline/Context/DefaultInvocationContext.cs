using System.Diagnostics;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Context
{
    /// <exclude />
    /// <summary>
    /// The <see cref="DefaultInvocationContext"/> provides a default implementation of <see cref="IInvocationContext"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationContext{TRequest, TResponse}"/>
    [DebuggerDisplay("{RequestType.Name} -> {ResponseType.Name}")]
    public abstract class DefaultInvocationContext : IInvocationContext
    {
        /// <summary>
        /// Initializes a new <see cref="DefaultInvocationContext"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null" />.</exception>
        public DefaultInvocationContext(object request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

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
    /// The <see cref="DefaultInvocationContext{TRequest, TResponse}"/> provides a default implementation of <see cref="IInvocationContext"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationContext{TRequest, TResponse}"/>
    [DebuggerDisplay("{typeof(TRequest).Name} -> {typeof(TResponse).Name}")]
    public class DefaultInvocationContext<TRequest, TResponse> : DefaultInvocationContext, IInvocationContext<TRequest, TResponse>, IInvocationContext<TResponse>
    {
        /// <summary>
        /// Initializes a new <see cref="DefaultInvocationContext{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null" />.</exception>
        public DefaultInvocationContext(TRequest request)
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
