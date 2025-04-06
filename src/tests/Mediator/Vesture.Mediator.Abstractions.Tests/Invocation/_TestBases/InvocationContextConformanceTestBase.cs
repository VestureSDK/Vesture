using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Vesture.Mediator.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Abstractions.Tests.Invocation
{
    public abstract class InvocationContextConformanceTestBase
    {
        protected abstract IInvocationContext<TRequest, TResponse> CreateInvocationContext<
            TRequest,
            TResponse
        >(TRequest request);

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
        public void RequestType_IsSameAsOriginalRequestType_WithPristineContext<
            TRequest,
            TResponse
        >(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var requestType = context.RequestType;

            // Assert
            Assert.That(requestType, Is.SameAs(request!.GetType()));
        }
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public void IsEvent_IsTrue<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isEvent = context.IsEvent;

            // Assert
            Assert.That(isEvent, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        [TestCaseSource_RequestResponse_NoResponse]
        public void IsEvent_IsFalse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isEvent = context.IsEvent;

            // Assert
            Assert.That(isEvent, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public void IsCommand_IsTrue<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isCommand = context.IsCommand;

            // Assert
            Assert.That(isCommand, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        [TestCaseSource_RequestResponse_NoResponse]
        public void IsCommand_IsFalse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isCommand = context.IsCommand;

            // Assert
            Assert.That(isCommand, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void IsRequest_IsTrue<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isRequest = context.IsRequest;

            // Assert
            Assert.That(isRequest, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        [TestCaseSource_RequestResponse_Event]
        [TestCaseSource_RequestResponse_NoResponse]
        public void IsRequest_IsFalse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isRequest = context.IsRequest;

            // Assert
            Assert.That(isRequest, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void HasResponseType_IsTrue<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var hasResponseType = context.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        [TestCaseSource_RequestResponse_Event]
        [TestCaseSource_RequestResponse_NoResponse]
        public void HasResponseType_IsFalse<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var hasResponseType = context.HasResponseType;

            // Assert
            Assert.That(hasResponseType, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void HasResponse_IsFalse_WhenNoResponseSet<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var hasResponse = context.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void HasResponse_IsTrue_WhenResponseSet<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            context.SetResponse(response);

            // Act
            var hasResponse = context.HasResponse;

            // Assert
            Assert.That(hasResponse, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void SetResponse_Response_IsTheOneSet_WhenNoResponseSetBefore<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            context.SetResponse(response);

            // Assert
            Assert.That(context.Response, Is.SameAs(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void SetResponse_Response_IsTheLastOneSet_WhenSettingMultipleResponses<
            TRequest,
            TResponse
        >(TRequest request, TResponse response)
            where TResponse : new()
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            context.SetResponse(response);

            var lastExpectedResponse = new TResponse();

            // Act
            context.SetResponse(lastExpectedResponse);

            // Assert
            Assert.That(context.Response, Is.SameAs(lastExpectedResponse));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        [TestCaseSource_RequestResponse_Unmarked]
        public void SetResponse_Response_IsUnset_WhenSettingNullResponse<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            context.SetResponse(response);

            // Act
            context.SetResponse(null);

            // Assert
            Assert.That(context.Response, Is.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
        public void Request_IsCtorValue_WithPristineContext<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var actualRequest = context.Request;

            // Assert
            Assert.That(actualRequest, Is.EqualTo(request));
        }
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void ResponseType_IsSetWithExpectedResponseType<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var responseType = context.ResponseType;

            // Assert
            Assert.That(responseType, Is.SameAs(typeof(TResponse)));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void Response_IsNull_WithPristineContext<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var actualResponse = context.Response;

            // Assert
            Assert.That(actualResponse, Is.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void Error_IsNull_WithPristineContext<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var error = context.Error;

            // Assert
            Assert.That(error, Is.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HasError_IsFalse_WhenErrorIsNull<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var hasError = context.HasError;

            // Assert
            Assert.That(hasError, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HasError_IsTrue_WhenErrorIsNotNull<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            context.SetError(new Exception("sample exception"));

            // Act
            var hasError = context.HasError;

            // Assert
            Assert.That(hasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void IsSuccess_IsTrue_WhenHasErrorIsFalse<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            // Act
            var isSuccess = context.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void IsSuccess_IsFalse_WhenHasErrorIsTrue<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            context.SetError(new Exception("sample exception"));

            // Act
            var isSuccess = context.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.False);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void SetError_Error_IsTheOneSet_WhenNoErrorSetBefore<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);
            var sampleError = new Exception("sample exception 1");

            // Act
            context.SetError(sampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(sampleError));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void SetError_Error_IsTheLastOneSet_WhenSettingMultipleErrors<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            var firstSampleError = new Exception("sample exception 1");
            context.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            context.SetError(lastSampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(lastSampleError));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void SetError_Error_IsUnset_WhenSettingNullError<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            context.SetError(new Exception("sample exception 1"));

            // Act
            context.SetError(null);

            // Assert
            Assert.That(context.Error, Is.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void AddError_Error_IsTheOneSet_WhenNoErrorSetBefore<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
            where TRequest : new()
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            var sampleError = new Exception("sample exception 1");

            // Act
            context.AddError(sampleError);

            // Assert
            Assert.That(context.Error, Is.SameAs(sampleError));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void AddError_Error_IsAggregated_WhenMultipleErrorsSet<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            var firstSampleError = new Exception("sample exception 1");
            context.SetError(firstSampleError);

            var lastSampleError = new Exception("sample exception 2");

            // Act
            context.AddError(lastSampleError);

            // Assert
            Assert.That(context.Error, Is.TypeOf<AggregateException>());
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void AddError_AggregatedErrors_AreAllTheExceptions<TRequest, TResponse>(
            TRequest request,
            TResponse response
        )
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

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

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void AddError_AggregatedErrors_AreAllTheExceptions_WhenPreviouslySetExceptionIsAggregated<
            TRequest,
            TResponse
        >(TRequest request, TResponse response)
        {
            // Arrange
            var context = CreateInvocationContext<TRequest, TResponse>(request);

            var firstSampleError = new Exception("sample exception 1");
            var secondSampleError = new Exception("sample exception 2");
            var aggregateError = new AggregateException(firstSampleError, secondSampleError);
            context.SetError(aggregateError);

            var lastSampleError = new Exception("sample exception 3");

            // Act
            context.AddError(lastSampleError);

            // Assert
            var error = (AggregateException)context.Error!;
            Assert.Multiple(() =>
            {
                Assert.That(error.InnerExceptions[0], Is.SameAs(firstSampleError));
                Assert.That(error.InnerExceptions[1], Is.SameAs(secondSampleError));
                Assert.That(error.InnerExceptions[2], Is.SameAs(lastSampleError));
            });
        }
    }
}
