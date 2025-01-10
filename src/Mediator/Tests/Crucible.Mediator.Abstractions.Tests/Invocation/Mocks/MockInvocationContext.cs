using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockInvocationContext<TRequest, TResponse> : IInvocationContext, IInvocationContext<TRequest, TResponse>
    {
        public Mock<IInvocationContext> Mock { get; } = new Mock<IInvocationContext>();

        private IInvocationContext _inner => Mock.Object;

        public MockInvocationContext(TRequest request)
            : this()
        {

#pragma warning disable CS8601 // Possible null reference assignment.
            Request = request;
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public MockInvocationContext()
        {
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
            IsRequest = !(IsEvent || IsCommand);
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

        public object Request
        {
            get => _inner.Request;
            set => Mock.SetupGet(m => m.Request).Returns(value);
        }

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

        public Type RequestType
        {
            get => _inner.RequestType;
            set => Mock.SetupGet(m => m.RequestType).Returns(value);
        }

        public bool IsEvent
        {
            get => _inner.IsEvent;
            set => Mock.SetupGet(m => m.IsEvent).Returns(value);
        }

        public bool IsCommand
        {
            get => _inner.IsCommand;
            set => Mock.SetupGet(m => m.IsCommand).Returns(value);
        }

        public bool IsRequest
        {
            get => _inner.IsRequest;
            set => Mock.SetupGet(m => m.IsRequest).Returns(value);
        }

        public bool HasResponseType
        {
            get => _inner.HasResponseType;
            set => Mock.SetupGet(m => m.HasResponseType).Returns(value);
        }

        public Type? ResponseType
        {
            get => _inner.ResponseType;
            set => Mock.SetupGet(m => m.ResponseType).Returns(value);
        }

        public bool HasResponse
        {
            get => _inner.HasResponse;
            set => Mock.SetupGet(m => m.HasResponse).Returns(value);
        }

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

        public bool HasError
        {
            get => _inner.HasError;
            set => Mock.SetupGet(m => m.HasError).Returns(value);
        }

        public bool IsSuccess
        {
            get => _inner.IsSuccess;
            set => Mock.SetupGet(m => m.IsSuccess).Returns(value);
        }

#pragma warning disable CS8603 // Possible null reference return.
        TRequest IInvocationContext<TRequest, TResponse>.Request => Response is TRequest tr ? tr : default;
#pragma warning restore CS8603 // Possible null reference return.

        TResponse? IInvocationContext<TResponse>.Response => Response is TResponse tr ? tr : default;

        public void SetError(Exception? error) => _inner.SetError(error);

        public void SetResponse(object? response) => _inner.SetResponse(response);

        public void AddError(Exception error) => _inner.AddError(error);
    }
}
