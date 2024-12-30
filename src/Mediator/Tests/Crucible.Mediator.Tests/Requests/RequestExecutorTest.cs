using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Crucible.Mediator.Tests.Requests.Mocks;

namespace Crucible.Mediator.Tests.Requests
{
    public class RequestExecutorTest : RequestExecutorTest<IRequestExecutor> { }

    public abstract class RequestExecutorTest<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRequestExecutor> : MediatorDiTestBase<TRequestExecutor>
        where TRequestExecutor: class, IRequestExecutor
    {
        protected IRequestExecutor RequestExecutor => Sut;

        protected MockRequestHandler<MockRequest, MockResponse> Handler { get; } = new MockRequestHandler<MockRequest, MockResponse>();

        protected MockRequest Request { get; } = new MockRequest();

        public RequestExecutorTest()
            : base()
        {
            MediatorDiBuilder
                .Request<MockRequest, MockResponse>()
                    .HandleWith(Handler);
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenRequestIsNull()
        {
            // Arrange
            // no arrange required

            // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => RequestExecutor.ExecuteAsync<MockResponse>(request: null, CancellationToken));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteAsync_Response_IsHandlerResponse()
        {
            // Arrange
            var expectedResponse = new MockResponse();
            Handler.MockExecute = (request) => expectedResponse;

            // Act
            MockResponse response = await RequestExecutor.ExecuteAsync(Request, CancellationToken);

            // Assert
            Assert.Same(expectedResponse, response);
        }

        [Fact]
        public async Task ExecuteAsync_Throws_WhenHandlerThrows()
        {
            // Arrange
            var expectedException = new Exception("sample exception");
            Handler.MockExecute = (_) => throw expectedException;

            // Act
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => RequestExecutor.ExecuteAsync(Request, CancellationToken));

            // Assert
            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task ExecuteAndCaptureAsync_ContextResponse_IsHandlerResponse()
        {
            // Arrange
            var expectedResponse = new MockResponse();
            Handler.MockExecute = (_) => expectedResponse;

            // Act
            IInvocationContext<MockResponse> invocationContext = await RequestExecutor.ExecuteAndCaptureAsync(Request, CancellationToken);

            // Assert
            Assert.Same(expectedResponse, invocationContext.Response);
        }

        [Fact]
        public async Task ExecuteAndCaptureAsync_ContextError_IsHandlerThrownException()
        {
            // Arrange
            var expectedException = new Exception("sample exception");
            Handler.MockExecute = (context) => throw expectedException;

            // Act
            IInvocationContext<MockResponse> invocationContext = await RequestExecutor.ExecuteAndCaptureAsync(Request, CancellationToken);

            // Assert
            Assert.Same(expectedException, invocationContext.Error);
        }
    }
}
