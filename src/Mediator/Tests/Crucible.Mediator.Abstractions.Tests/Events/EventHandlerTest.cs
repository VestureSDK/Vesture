using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Events
{
    [SampleTest]
    [TestFixtureSource_Request_Events]
    public class EventHandlerTest<TEvent> : InvocationHandlerConformanceTestBase<TEvent, EventResponse, EventHandlerTest<TEvent>.SampleEventHandler>
    {
        protected Mock<IEventHandlerLifeCycle> LifeCycle { get; } = new();

        public EventHandlerTest(TEvent @event)
            : base(@event) { }

        protected override SampleEventHandler CreateInvocationHandler() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<TEvent>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<TEvent, EventResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<TEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        public interface IEventHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(TEvent @event, CancellationToken cancellationToken);
        }

        public class SampleEventHandler : Mediator.Events.EventHandler<TEvent>
        {
            private readonly IEventHandlerLifeCycle _lifeCycle;

            public SampleEventHandler(IEventHandlerLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(@event, cancellationToken);
            }
        }
    }
}
