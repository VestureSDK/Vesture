using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Mocks.Invocation
{
    /// <summary>
    /// Defines a mock <see cref="IInvocationContext{TRequest, TResponse}"/> contract.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public class MockInvocationContext<TRequest, TResponse> : IInvocationContext, IInvocationContext<TRequest, TResponse>
    {
        /// <summary>
        /// The <see cref="Mock{T}"/> instance.
        /// </summary>
        public Mock<IInvocationContext> Mock { get; } = new Mock<IInvocationContext>();

        private IInvocationContext _inner => Mock.Object;

        /// <summary>
        /// Initializes a new <see cref="MockInvocationContext{TRequest, TResponse}"/> instance.
        /// </summary>
        public MockInvocationContext()
        {
            Request = default!;

            Mock.Setup(m => m.SetResponse(It.IsAny<object>())).Callback<object>(response => Response = response);
            Mock.Setup(m => m.SetError(It.IsAny<Exception>())).Callback<Exception>(error => Error = error);
            Mock.Setup(m => m.AddError(It.IsAny<Exception>())).Callback<Exception>(error =>
            {
                if (Error is null)
                {
                    SetError(error);
                }
                else
                {
                    var errors = AggregateErrors(Error);
                    errors.Add(error);
                    Error = new AggregateException(errors);
                }
            });

            RequestType = typeof(TRequest);
            ResponseType = typeof(TResponse);

            IsSuccess = true;

            IsEvent = ResponseType == EventResponse.Type;
            IsCommand = ResponseType == CommandResponse.Type;
            IsRequest = !(IsEvent || IsCommand || ResponseType == NoResponse.Type);
            HasResponseType = IsRequest;
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

        /// <inheritdoc />
        public object Request
        {
            get => _inner.Request;
            set => Mock.SetupGet(m => m.Request).Returns(value);
        }

        /// <inheritdoc />
        public object? Response
        {
            get => _inner.Response;
            set
            {
                Mock.SetupGet(m => m.Response).Returns(value);
                if (Response != null)
                {
                    HasResponse = true;
                }
            }
        }

        /// <inheritdoc />
        public Type RequestType
        {
            get => _inner.RequestType;
            set => Mock.SetupGet(m => m.RequestType).Returns(value);
        }

        /// <inheritdoc />
        public bool IsEvent
        {
            get => _inner.IsEvent;
            set => Mock.SetupGet(m => m.IsEvent).Returns(value);
        }

        /// <inheritdoc />
        public bool IsCommand
        {
            get => _inner.IsCommand;
            set => Mock.SetupGet(m => m.IsCommand).Returns(value);
        }

        /// <inheritdoc />
        public bool IsRequest
        {
            get => _inner.IsRequest;
            set => Mock.SetupGet(m => m.IsRequest).Returns(value);
        }

        /// <inheritdoc />
        public bool HasResponseType
        {
            get => _inner.HasResponseType;
            set => Mock.SetupGet(m => m.HasResponseType).Returns(value);
        }

        /// <inheritdoc />
        public Type? ResponseType
        {
            get => _inner.ResponseType;
            set => Mock.SetupGet(m => m.ResponseType).Returns(value);
        }

        /// <inheritdoc />
        public bool HasResponse
        {
            get => _inner.HasResponse;
            set => Mock.SetupGet(m => m.HasResponse).Returns(value);
        }

        /// <inheritdoc />
        public Exception? Error
        {
            get => _inner.Error;
            set
            {
                Mock.SetupGet(m => m.Error).Returns(value);
                if (Error != null)
                {
                    HasError = true;
                    IsSuccess = false;
                }
                else
                {
                    HasError = false;
                    IsSuccess = true;
                }
            }
        }

        /// <inheritdoc />
        public bool HasError
        {
            get => _inner.HasError;
            set => Mock.SetupGet(m => m.HasError).Returns(value);
        }

        /// <inheritdoc />
        public bool IsSuccess
        {
            get => _inner.IsSuccess;
            set => Mock.SetupGet(m => m.IsSuccess).Returns(value);
        }

#pragma warning disable CS8603 // Possible null reference return.
        /// <inheritdoc />
        TRequest IInvocationContext<TRequest, TResponse>.Request => Request is TRequest tr ? tr : default;
#pragma warning restore CS8603 // Possible null reference return.

        /// <inheritdoc />
        TResponse? IInvocationContext<TResponse>.Response => Response is TResponse tr ? tr : default;

        /// <inheritdoc />
        public void SetError(Exception? error) => _inner.SetError(error);

        /// <inheritdoc />
        public void SetResponse(object? response) => _inner.SetResponse(response);

        /// <inheritdoc />
        public void AddError(Exception error) => _inner.AddError(error);
    }
}
