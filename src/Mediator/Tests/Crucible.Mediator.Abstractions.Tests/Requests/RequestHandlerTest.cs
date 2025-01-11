using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Moq;
using static Crucible.Mediator.Abstractions.Tests.Requests.RequestHandlerTest;

namespace Crucible.Mediator.Abstractions.Tests.Requests
{
    [SampleTest]
    public class RequestHandlerTest : InvocationHandlerConformanceTestBase<MockRequest, MockResponse, SampleRequestHandler>
    {
        protected Mock<IRequestHandlerLifeCycle> LifeCycle { get; } = new();

        protected MockResponse Response { get; set; } = new();

        public RequestHandlerTest()
            : base(new()) { }

        protected override SampleRequestHandler CreateInvocationHandler() => new(LifeCycle.Object, Response);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<MockRequest>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<MockRequest, MockResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<MockRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        [Test]
        public async Task HandleAsync_ReturnsExpectedResponse()
        {
            // Arrange
            // No arrange required

            // Act
            var response = await ((IInvocationHandler<MockRequest, MockResponse>)Handler).HandleAsync(Request, CancellationToken);

            // Assert
            Assert.That(response, Is.EqualTo(Response));
        }

        public interface IRequestHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(MockRequest request, CancellationToken cancellationToken);
        }

        public class SampleRequestHandler : CommandHandler<MockRequest, MockResponse>
        {
            private readonly IRequestHandlerLifeCycle _lifeCycle;

            private readonly MockResponse _response;

            public SampleRequestHandler(IRequestHandlerLifeCycle lifeCycle, MockResponse response)
            {
                _lifeCycle = lifeCycle;
                _response = response;
            }

            public override async Task<MockResponse> HandleAsync(MockRequest request, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(request, cancellationToken);
                return _response;
            }
        }
    }
}
