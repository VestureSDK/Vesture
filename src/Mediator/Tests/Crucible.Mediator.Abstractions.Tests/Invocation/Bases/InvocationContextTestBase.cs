using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
    public abstract class InvocationContextTestBase_Event<TRequest, TContext>
        : InvocationContextTestBase<TRequest, EventResponse, TContext>
        where TContext : IInvocationContext<TRequest, EventResponse>
    {
        protected InvocationContextTestBase_Event(TRequest defaultRequest)
            : base(defaultRequest) { }

        [Test]
        public void IsEvent_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.True, message: $"{nameof(IInvocationContext<TRequest, EventResponse>.IsEvent)} should be {true}");
        }

        [Test]
        public void IsCommand_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False, message: $"{nameof(IInvocationContext<TRequest, EventResponse>.IsCommand)} should be {false}");
        }

        [Test]
        public void IsRequest_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False, message: $"{nameof(IInvocationContext<TRequest, EventResponse>.IsRequest)} should be {false}");
        }

        [Test]
        public void HasResponseType_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False, message: $"{nameof(IInvocationContext<TRequest, EventResponse>.HasResponseType)} should be {false}");
        }
    }

    public abstract class InvocationContextTestBase_Command<TRequest, TContext>
        : InvocationContextTestBase<TRequest, CommandResponse, TContext>
        where TContext : IInvocationContext<TRequest, CommandResponse>
    {
        protected InvocationContextTestBase_Command(TRequest defaultRequest)
            : base(defaultRequest) { }

        [Test]
        public void IsEvent_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False, message: $"{nameof(IInvocationContext<TRequest, CommandResponse>.IsEvent)} should be {false}");
        }

        [Test]
        public void IsCommand_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.True, message: $"{nameof(IInvocationContext<TRequest, CommandResponse>.IsCommand)} should be {true}");
        }

        [Test]
        public void IsRequest_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False, message: $"{nameof(IInvocationContext<TRequest, CommandResponse>.IsRequest)} should be {false}");
        }

        [Test]
        public void HasResponseType_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False, message: $"{nameof(IInvocationContext<TRequest, CommandResponse>.HasResponseType)} should be {false}");
        }
    }

    public abstract class InvocationContextTestBase_Request<TRequest, TResponse, TContext>
        : InvocationContextTestBase<TRequest, TResponse, TContext>
        where TContext : IInvocationContext<TRequest, TResponse>
    {
        protected InvocationContextTestBase_Request(TRequest defaultRequest)
            : base(defaultRequest) { }

        protected abstract TResponse CreateResponse();

        [Test]
        public void IsEvent_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isEvent = InvocationContext.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False, message: $"{nameof(IInvocationContext<TRequest, TResponse>.IsEvent)} should be {false}");
        }

        [Test]
        public void IsCommand_IsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isCommand = InvocationContext.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False, message: $"{nameof(IInvocationContext<TRequest, TResponse>.IsCommand)} should be {false}");
        }

        [Test]
        public void IsRequest_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var isRequest = InvocationContext.IsRequest;

            // Assert
            Assert.That(isRequest, Is.True, message: $"{nameof(IInvocationContext<TRequest, TResponse>.IsRequest)} should be {true}");
        }

        [Test]
        public void HasResponseType_IsTrue()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponseType = InvocationContext.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.True, message: $"{nameof(IInvocationContext<TRequest, TResponse>.HasResponseType)} should be {true}");
        }

        [Test]
        public void HasResponse_IsFalse_WhenNoResponseSet()
        {
            // Arrange
            // No arrange required

            // Act
            var hasResponse = InvocationContext.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.False, message: $"{nameof(IInvocationContext<TRequest, TResponse>.HasResponse)} should be {false} when no response is set");
        }

        [Test]
        public void HasResponse_IsTrue_WhenResponseSet()
        {
            // Arrange
            InvocationContext.SetResponse(CreateResponse());

            // Act
            var hasResponse = InvocationContext.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.True, message: $"{nameof(IInvocationContext<TRequest, TResponse>.HasResponse)} should be {true} when a response is set");
        }

        [Test]
        public void SetResponse_Response_IsTheOneSet_WhenNoResponseSetBefore()
        {
            // Arrange
            var sampleResponse = CreateResponse();

            // Act
            InvocationContext.SetResponse(sampleResponse);

            // Assert
            Assert.That(InvocationContext.Response, Is.SameAs(sampleResponse), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Response)} should be same as response set with {nameof(IInvocationContext<TRequest, TResponse>.SetResponse)} when no response set.");
        }

        [Test]
        public void SetResponse_Response_IsTheLastOneSet_WhenSettingMultipleResponses()
        {
            // Arrange
            var firstSampleResponse = CreateResponse();
            InvocationContext.SetResponse(firstSampleResponse);

            var lastSampleResponse = CreateResponse();

            // Act
            InvocationContext.SetResponse(lastSampleResponse);

            // Assert
            Assert.That(InvocationContext.Response, Is.SameAs(lastSampleResponse), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Response)} should be same as the last response set with {nameof(IInvocationContext<TRequest, TResponse>.SetResponse)} when multiple response set.");
        }

        [Test]
        public void SetResponse_Response_IsUnset_WhenSettingNullResponse()
        {
            // Arrange
            InvocationContext.SetResponse(new object());

            // Act
            InvocationContext.SetResponse(null);

            // Assert
            Assert.That(InvocationContext.Response, Is.Null, message: $"{nameof(IInvocationContext<TRequest, TResponse>.Response)} should be null when calling {nameof(IInvocationContext<TRequest, TResponse>.SetResponse)} with null.");
        }
    }

    public abstract class InvocationContextTestBase<TRequest, TResponse, TContext>
        where TContext: IInvocationContext<TRequest, TResponse>
    {
        protected TRequest Request { get; set; }

        protected Lazy<TContext> InvocationContextInitializer { get; }

        protected TContext InvocationContext => InvocationContextInitializer.Value;

        public InvocationContextTestBase(TRequest defaultRequest)
        {
            Request = defaultRequest;

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            InvocationContextInitializer = new Lazy<TContext>(() => CreateInvocationContext(Request!));
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TContext CreateInvocationContext(TRequest request);

        [Test]
        public void RequestType_IsSameAsOriginalRequestType_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var requestType = InvocationContext.RequestType;

            // Assert
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
            Assert.That(requestType, Is.SameAs(Request!.GetType()), message: $"{nameof(IInvocationContext<TRequest, TResponse>.RequestType)} should be equal to ctor value GetType with a pristine context");
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint
        }

        [Test]
        public void Request_IsCtorValue_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var request = ((IInvocationContext)InvocationContext).Request;

            // Assert
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
            Assert.That(request, Is.SameAs(Request), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Request)} should be equal to ctor value with a pristine context");
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint
        }

        [Test]
        public void ResponseType_IsSetWithExpectedResponseType()
        {
            // Arrange
            // No arrange required

            // Act
            var responseType = InvocationContext.ResponseType;

            // Assert
            Assert.That(responseType, Is.SameAs(typeof(TResponse)), message: $"{nameof(IInvocationContext<TRequest, TResponse>.ResponseType)} should be the expected type");
        }

        [Test]
        public void Response_IsNull_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var response = InvocationContext.Response;

            // Assert
            Assert.That(response, Is.Null, message: $"{nameof(IInvocationContext<TRequest, TResponse>.Response)} should be null with a pristine context");
        }

        [Test]
        public void Error_IsNull_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var error = InvocationContext.Error;

            // Assert
            Assert.That(error, Is.Null, message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be null with a pristine context");
        }

        [Test]
        public void HasError_IsFalse_WhenErrorIsNull()
        {
            // Arrange
            // No arrange required

            // Act
            var hasError = InvocationContext.HasError;

            // Assert
            Assert.That(hasError, Is.False, message: $"{nameof(IInvocationContext<TRequest, TResponse>.HasError)} should be {false} when no error has been set with {nameof(IInvocationContext<TRequest, TResponse>.SetError)}");
        }

        [Test]
        public void HasError_IsTrue_WhenErrorIsNotNull()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception"));

            // Act
            var hasError = InvocationContext.HasError;

            // Assert
            Assert.That(hasError, Is.True, message: $"{nameof(IInvocationContext<TRequest, TResponse>.HasError)} should be {true} when an error has been set with {nameof(IInvocationContext<TRequest, TResponse>.SetError)}");
        }

        [Test]
        public void IsSuccess_IsTrue_WhenHasErrorIsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isSuccess = InvocationContext.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.True, message: $"{nameof(IInvocationContext<TRequest, TResponse>.IsSuccess)} should be {true} when {nameof(IInvocationContext<TRequest, TResponse>.HasError)} is {false}");
        }

        [Test]
        public void IsSuccess_IsFalse_WhenHasErrorIsTrue()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception"));

            // Act
            var isSuccess = InvocationContext.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.False, message: $"{nameof(IInvocationContext<TRequest, TResponse>.IsSuccess)} should be {false} when {nameof(IInvocationContext<TRequest, TResponse>.HasError)} is {true}");
        }

        [Test]
        public void SetError_Error_IsTheOneSet_WhenNoErrorSetBefore()
        {
            // Arrange
            var sampleError = new Exception("sample exception 1");

            // Act
            InvocationContext.SetError(sampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(sampleError), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be same as error set with {nameof(IInvocationContext<TRequest, TResponse>.SetError)} when no errors set.");
        }

        [Test]
        public void SetError_Error_IsTheLastOneSet_WhenSettingMultipleErrors()
        {
            // Arrange
            var firstSampleError = new Exception("sample exception 1");
            InvocationContext.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            InvocationContext.SetError(lastSampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(lastSampleError), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be same as the last error set with {nameof(IInvocationContext<TRequest, TResponse>.SetError)} when multiple errors set.");
        }

        [Test]
        public void SetError_Error_IsUnset_WhenSettingNullError()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception 1"));

            // Act
            InvocationContext.SetError(null);

            // Assert
            Assert.That(InvocationContext.Error, Is.Null, message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be null when calling {nameof(IInvocationContext<TRequest, TResponse>.SetError)} with null.");
        }

        [Test]
        public void AddError_Error_IsTheOneSet_WhenNoErrorSetBefore()
        {
            // Arrange
            var sampleError = new Exception("sample exception 1");

            // Act
            InvocationContext.AddError(sampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(sampleError), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be same as error set with {nameof(IInvocationContext<TRequest, TResponse>.AddError)} when no errors set.");
        }

        [Test]
        public void AddError_Error_IsAggregated_WhenMultipleErrorsSet()
        {
            // Arrange
            var firstSampleError = new Exception("sample exception 1");
            InvocationContext.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            InvocationContext.AddError(lastSampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.TypeOf<AggregateException>(), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} should be an {typeof(AggregateException)} when multiple errors occured.");
        }

        [Test]
        public void AddError_AggregatedErrors_AreAllTheExceptions()
        {
            // Arrange
            var firstSampleError = new Exception("sample exception 1");
            var lastSampleError = new Exception("sample exception 2");

            // Act
            InvocationContext.AddError(firstSampleError);
            InvocationContext.AddError(lastSampleError);

            // Assert
            var error = (AggregateException)InvocationContext.Error!;
            Assert.Multiple(() =>
            {
                Assert.That(error.InnerExceptions[0], Is.SameAs(firstSampleError), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} as an {typeof(AggregateException)} should contain at [0] the first exception.");
                Assert.That(error.InnerExceptions[1], Is.SameAs(lastSampleError), message: $"{nameof(IInvocationContext<TRequest, TResponse>.Error)} as an {typeof(AggregateException)} should contain at [1] the second exception.");
            });
        }
    }
}
