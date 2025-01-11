using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
    public abstract class InvocationContextConformanceTestBase
    {
        protected abstract IInvocationContext<TRequest, TResponse> CreateInvocationContext<TRequest, TResponse>(TRequest request);

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
        public void RequestType_IsSameAsOriginalRequestType_WithPristineContext<TRequest, TResponse>()
            where TRequest: new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var requestType = context.RequestType;

            // Assert
            Assert.That(requestType, Is.SameAs(expectedRequest!.GetType()));
        }
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        public void IsEvent_IsTrue<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isEvent = context.IsEvent;

            // Assert
            Assert.That(isEvent, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void IsEvent_IsFalse<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isEvent = context.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        public void IsCommand_IsTrue<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isCommand = context.IsCommand;

            // Assert
            Assert.That(isCommand, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void IsCommand_IsFalse<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isCommand = context.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void IsRequest_IsTrue<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isRequest = context.IsRequest;

            // Assert
            Assert.That(isRequest, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void IsRequest_IsFalse<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isRequest = context.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void HasResponseType_IsTrue<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var hasResponseType = context.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void HasResponseType_IsFalse<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var hasResponseType = context.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void HasResponse_IsFalse_WhenNoResponseSet<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var hasResponse = context.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void HasResponse_IsTrue_WhenResponseSet<TRequest, TResponse>()
            where TRequest : new()
            where TResponse : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var expectedResponse = new TResponse();
            context.SetResponse(expectedResponse);

            // Act
            var hasResponse = context.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void SetResponse_Response_IsTheOneSet_WhenNoResponseSetBefore<TRequest, TResponse>()
            where TRequest : new()
            where TResponse : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var expectedResponse = new TResponse();

            // Act
            context.SetResponse(expectedResponse);

            // Assert
            Assert.That(context.Response, Is.SameAs(expectedResponse));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void SetResponse_Response_IsTheLastOneSet_WhenSettingMultipleResponses<TRequest, TResponse>()
            where TRequest : new()
            where TResponse : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var firstExpectedResponse = new TResponse();
            context.SetResponse(firstExpectedResponse);

            var lastExpectedResponse = new TResponse();

            // Act
            context.SetResponse(lastExpectedResponse);

            // Assert
            Assert.That(context.Response, Is.SameAs(lastExpectedResponse));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        public void SetResponse_Response_IsUnset_WhenSettingNullResponse<TRequest, TResponse>()
            where TRequest : new()
            where TResponse : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var expectedResponse = new TResponse();
            context.SetResponse(expectedResponse);

            // Act
            context.SetResponse(null);

            // Assert
            Assert.That(context.Response, Is.Null);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
        public void Request_IsCtorValue_WithPristineContext<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var request = ((IInvocationContext)context).Request;

            // Assert
            Assert.That(request, Is.SameAs(expectedRequest));
        }
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void ResponseType_IsSetWithExpectedResponseType<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var responseType = context.ResponseType;

            // Assert
            Assert.That(responseType, Is.SameAs(typeof(TResponse)));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void Response_IsNull_WithPristineContext<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var response = context.Response;

            // Assert
            Assert.That(response, Is.Null);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void Error_IsNull_WithPristineContext<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var error = context.Error;

            // Assert
            Assert.That(error, Is.Null);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void HasError_IsFalse_WhenErrorIsNull<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var hasError = context.HasError;

            // Assert
            Assert.That(hasError, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void HasError_IsTrue_WhenErrorIsNotNull<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);
            context.SetError(new Exception("sample exception"));

            // Act
            var hasError = context.HasError;

            // Assert
            Assert.That(hasError, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void IsSuccess_IsTrue_WhenHasErrorIsFalse<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            // Act
            var isSuccess = context.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void IsSuccess_IsFalse_WhenHasErrorIsTrue<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);
            context.SetError(new Exception("sample exception"));

            // Act
            var isSuccess = context.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.False);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void SetError_Error_IsTheOneSet_WhenNoErrorSetBefore<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);
            var sampleError = new Exception("sample exception 1");

            // Act
            context.SetError(sampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(sampleError));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void SetError_Error_IsTheLastOneSet_WhenSettingMultipleErrors<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var firstSampleError = new Exception("sample exception 1");
            context.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            context.SetError(lastSampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(lastSampleError));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void SetError_Error_IsUnset_WhenSettingNullError<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            context.SetError(new Exception("sample exception 1"));

            // Act
            context.SetError(null);

            // Assert
            Assert.That(context.Error, Is.Null);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void AddError_Error_IsTheOneSet_WhenNoErrorSetBefore<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var sampleError = new Exception("sample exception 1");

            // Act
            context.AddError(sampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(sampleError));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void AddError_Error_IsAggregated_WhenMultipleErrorsSet<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var firstSampleError = new Exception("sample exception 1");
            context.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            context.AddError(lastSampleError);

            // Assert
            Assert.That(context.Error, Is.TypeOf<AggregateException>());
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute<MockRequest, MockResponse>]
        [TestCaseGenericNoParamsAttribute<MockEvent, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockCommand, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, MockUnmarked>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, EventResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, CommandResponse>]
        [TestCaseGenericNoParamsAttribute<MockUnmarked, NoResponse>]
        public void AddError_AggregatedErrors_AreAllTheExceptions<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var expectedRequest = new TRequest();
            var context = CreateInvocationContext<TRequest, TResponse>(expectedRequest);

            var firstSampleError = new Exception("sample exception 1");
            var lastSampleError = new Exception("sample exception 2");

            // Act
            context.AddError(firstSampleError);
            context.AddError(lastSampleError);

            // Assert
            var error = (AggregateException)context.Error!;
            Assert.Multiple(() =>
            {
                Assert.That(error.InnerExceptions[0], Is.SameAs(firstSampleError));
                Assert.That(error.InnerExceptions[1], Is.SameAs(lastSampleError));
            });
        }
    }
}
