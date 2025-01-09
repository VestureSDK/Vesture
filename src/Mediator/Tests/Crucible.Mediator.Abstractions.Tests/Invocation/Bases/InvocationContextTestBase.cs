using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
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
            var request = InvocationContext.Request;

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
