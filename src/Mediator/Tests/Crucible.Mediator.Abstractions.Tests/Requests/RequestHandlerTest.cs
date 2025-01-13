using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Requests
{
    [SampleTest]
    [TestFixtureSource_RequestResponse_RequestAttribute]
    public class RequestHandlerTest<TRequest, TResponse> : InvocationHandlerConformanceTestBase<TRequest, TResponse, RequestHandlerTest<TRequest, TResponse>.SampleRequestHandler>
        where TRequest : IRequest<TResponse>
    {
        protected Mock<IRequestHandlerLifeCycle> LifeCycle { get; } = new();

        protected TResponse Response { get; set; }

        public RequestHandlerTest(TRequest request, TResponse response)
            : base(request)
        {
            Response = response;
        }

        protected override SampleRequestHandler CreateInvocationHandler() => new(LifeCycle.Object, Response);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<TRequest, TResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);

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
            var response = await ((IInvocationHandler<TRequest, TResponse>)Handler).HandleAsync(Request, CancellationToken);

            // Assert
            Assert.That(response, Is.EqualTo(Response));
        }

        public interface IRequestHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(TRequest request, CancellationToken cancellationToken);
        }

        public class SampleRequestHandler : CommandHandler<TRequest, TResponse>
        {
            private readonly IRequestHandlerLifeCycle _lifeCycle;

            private readonly TResponse _response;

            public SampleRequestHandler(IRequestHandlerLifeCycle lifeCycle, TResponse response)
            {
                _lifeCycle = lifeCycle;
                _response = response;
            }

            public override async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(request, cancellationToken);
                return _response;
            }
        }
    }
}
