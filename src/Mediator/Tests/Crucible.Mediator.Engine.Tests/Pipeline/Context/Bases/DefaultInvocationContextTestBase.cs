using Crucible.Mediator.Engine.Pipeline.Context;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases
{
    public class DefaultInvocationContextTestBase<TRequest, TResponse>
    {
        protected TRequest Request { get; set; }

        protected Lazy<DefaultInvocationContext<TRequest, TResponse>> InvocationContextInitializer { get; }

        protected DefaultInvocationContext<TRequest, TResponse> InvocationContext => InvocationContextInitializer.Value;

        public DefaultInvocationContextTestBase(TRequest defaultRequest)
        {
            Request = defaultRequest;
            InvocationContextInitializer = new Lazy<DefaultInvocationContext<TRequest, TResponse>>(() => new DefaultInvocationContext<TRequest, TResponse>(Request));
        }

        [Test]
        public void RequestType_IsSameAsOriginalRequestType_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var requestType = InvocationContext.RequestType;

            // Assert
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
            Assert.That(requestType, Is.SameAs(Request!.GetType()), message: $"{nameof(DefaultInvocationContext.RequestType)} should be equal to ctor value GetType with a pristine context");
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint
        }

        [Test]
        public void Request_IsCtorValue_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var request = InvocationContext.Request;

            // Assert
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint
            Assert.That(request, Is.SameAs(Request), message: $"{nameof(DefaultInvocationContext.Request)} should be equal to ctor value with a pristine context");
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
            Assert.That(responseType, Is.SameAs(typeof(TResponse)), message: $"{nameof(DefaultInvocationContext.ResponseType)} should be the expected type");
        }

        [Test]
        public void Response_IsNull_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var response = InvocationContext.Response;

            // Assert
            Assert.That(response, Is.Null, message: $"{nameof(DefaultInvocationContext.Response)} should be null with a pristine context");
        }

        [Test]
        public void Error_IsNull_WithPristineContext()
        {
            // Arrange
            // No arrange required

            // Act
            var error = InvocationContext.Error;

            // Assert
            Assert.That(error, Is.Null, message: $"{nameof(DefaultInvocationContext.Error)} should be null with a pristine context");
        }

        [Test]
        public void HasError_IsFalse_WhenErrorIsNull()
        {
            // Arrange
            // No arrange required

            // Act
            var hasError = InvocationContext.HasError;

            // Assert
            Assert.That(hasError, Is.False, message: $"{nameof(DefaultInvocationContext.HasError)} should be {false} when no error has been set with {nameof(DefaultInvocationContext.SetError)}");
        }

        [Test]
        public void HasError_IsTrue_WhenErrorIsNotNull()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception"));

            // Act
            var hasError = InvocationContext.HasError;

            // Assert
            Assert.That(hasError, Is.True, message: $"{nameof(DefaultInvocationContext.HasError)} should be {true} when an error has been set with {nameof(DefaultInvocationContext.SetError)}");
        }

        [Test]
        public void IsSuccess_IsTrue_WhenHasErrorIsFalse()
        {
            // Arrange
            // No arrange required

            // Act
            var isSuccess = InvocationContext.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.True, message: $"{nameof(DefaultInvocationContext.IsSuccess)} should be {true} when {nameof(DefaultInvocationContext.HasError)} is {false}");
        }

        [Test]
        public void IsSuccess_IsFalse_WhenHasErrorIsTrue()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception"));

            // Act
            var isSuccess = InvocationContext.IsSuccess;

            // Assert
            Assert.That(isSuccess, Is.False, message: $"{nameof(DefaultInvocationContext.IsSuccess)} should be {false} when {nameof(DefaultInvocationContext.HasError)} is {true}");
        }

        [Test]
        public void SetError_Error_IsTheOneSet_WhenNoErrorSetBefore()
        {
            // Arrange
            var sampleError = new Exception("sample exception 1");

            // Act
            InvocationContext.SetError(sampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(sampleError), message: $"{nameof(DefaultInvocationContext.Error)} should be same as error set with {nameof(DefaultInvocationContext.SetError)} when no errors set.");
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
            Assert.That(InvocationContext.Error, Is.SameAs(lastSampleError), message: $"{nameof(DefaultInvocationContext.Error)} should be same as the last error set with {nameof(DefaultInvocationContext.SetError)} when multiple errors set.");
        }

        [Test]
        public void SetError_Error_IsUnset_WhenSettingNullError()
        {
            // Arrange
            InvocationContext.SetError(new Exception("sample exception 1"));

            // Act
            InvocationContext.SetError(null);

            // Assert
            Assert.That(InvocationContext.Error, Is.Null, message: $"{nameof(DefaultInvocationContext.Error)} should be null when calling {nameof(DefaultInvocationContext.SetError)} with null.");
        }

        [Test]
        public void AddError_Error_IsTheOneSet_WhenNoErrorSetBefore()
        {
            // Arrange
            var sampleError = new Exception("sample exception 1");

            // Act
            InvocationContext.AddError(sampleError);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(sampleError), message: $"{nameof(DefaultInvocationContext.Error)} should be same as error set with {nameof(DefaultInvocationContext.AddError)} when no errors set.");
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
            Assert.That(InvocationContext.Error, Is.TypeOf<AggregateException>(), message: $"{nameof(DefaultInvocationContext.Error)} should be an {typeof(AggregateException)} when multiple errors occured.");
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
                Assert.That(error.InnerExceptions[0], Is.SameAs(firstSampleError), message: $"{nameof(DefaultInvocationContext.Error)} as an {typeof(AggregateException)} should contain at [0] the first exception.");
                Assert.That(error.InnerExceptions[1], Is.SameAs(lastSampleError), message: $"{nameof(DefaultInvocationContext.Error)} as an {typeof(AggregateException)} should contain at [1] the second exception.");
            });
        }
    }
}
