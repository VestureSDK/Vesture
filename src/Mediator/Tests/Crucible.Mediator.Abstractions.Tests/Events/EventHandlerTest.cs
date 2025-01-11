using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Moq;
using static Crucible.Mediator.Abstractions.Tests.Events.EventHandlerTest;

namespace Crucible.Mediator.Abstractions.Tests.Events
{
    [SampleTest]
    public class EventHandlerTest : InvocationHandlerConformanceTestBase<MockEvent, EventResponse, SampleEventHandler>
    {
        protected Mock<IEventHandlerLifeCycle> LifeCycle { get; } = new();

        public EventHandlerTest()
            : base(new()) { }

        protected override SampleEventHandler CreateInvocationHandler() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<MockEvent, EventResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<MockEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        public interface IEventHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(MockEvent @event, CancellationToken cancellationToken);
        }

        public class SampleEventHandler : Mediator.Events.EventHandler<MockEvent>
        {
            private readonly IEventHandlerLifeCycle _lifeCycle;

            public SampleEventHandler(IEventHandlerLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(MockEvent @event, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(@event, cancellationToken);
            }
        }
    }
}
