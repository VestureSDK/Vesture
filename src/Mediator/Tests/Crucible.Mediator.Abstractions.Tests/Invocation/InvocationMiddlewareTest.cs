using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Invocation;
using Moq;
using static Crucible.Mediator.Abstractions.Tests.Invocation.InvocationMiddlewareTest;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public class InvocationMiddlewareTest : InvocationMiddlewareTestBase<MockContract, MockContract, SampleInvocationMiddleware>
    {
        protected Mock<IInvocationMiddlewareLifeCycle> LifeCycle { get; } = new();

        public InvocationMiddlewareTest()
            : base(new()) { }

        protected override SampleInvocationMiddleware CreateMiddleware() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes_AreInSequence()
        {
            // Arrange
            var entersOnBeforeNextAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersOnBeforeNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()))
                .Returns(async () => await entersOnBeforeNextAsyncTaskCompletionSource.Task);

            var entersOnAfterNextAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersOnAfterNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()))
                .Returns(async () => await entersOnAfterNextAsyncTaskCompletionSource.Task);

            var nextAsyncTaskCompletionSource = new TaskCompletionSource();
            var nextExecuted = false;
            Next = (ct) =>
            {
                nextExecuted = true;
                return nextAsyncTaskCompletionSource.Task;
            };

            // Act / Assert
            var task = Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersOnBeforeNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(nextExecuted, Is.False);
            LifeCycle.Verify(m => m.InnerEntersOnAfterNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Never);

            entersOnBeforeNextAsyncTaskCompletionSource.SetResult();

            LifeCycle.Verify(m => m.InnerEntersOnBeforeNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(nextExecuted, Is.True);
            LifeCycle.Verify(m => m.InnerEntersOnAfterNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Never);

            nextAsyncTaskCompletionSource.SetResult();

            LifeCycle.Verify(m => m.InnerEntersOnBeforeNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(nextExecuted, Is.True);
            LifeCycle.Verify(m => m.InnerEntersOnAfterNextAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersOnAfterNextAsyncTaskCompletionSource.SetResult();
            await task;
        }

        [Test]
        public async Task HandleAsync_OnErrorAsync_IsInvokedWhenContextHasError()
        {
            // Arrange
            Next = (ct) =>
            {
                InvocationContext.SetError(new Exception("sample error"));
                return Task.CompletedTask;
            };

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            LifeCycle.Verify(m => m.InnerEntersOnErrorAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_OnErrorAsync_IsNotInvokedWhenContextIsValid()
        {
            // Arrange
            // No arrange required

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            LifeCycle.Verify(m => m.InnerEntersOnErrorAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task HandleAsync_OnSuccessAsync_IsInvokedWhenContextIsValid()
        {
            // Arrange
            // No arrange required

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            LifeCycle.Verify(m => m.InnerEntersOnSucessAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_OnSuccessAsync_IsNotInvokedWhenContextHasError()
        {
            // Arrange
            Next = (ct) =>
            {
                InvocationContext.SetError(new Exception("sample error"));
                return Task.CompletedTask;
            };

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            LifeCycle.Verify(m => m.InnerEntersOnSucessAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        public interface IInvocationMiddlewareLifeCycle
        {
            Task InnerEntersHandleAsync(IInvocationContext<MockContract, MockContract> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken);

            Task InnerEntersOnBeforeNextAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken);

            Task InnerEntersOnAfterNextAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken);

            Task InnerEntersOnErrorAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken);

            Task InnerEntersOnSucessAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken);
        }

        public class SampleInvocationMiddleware : InvocationMiddleware<MockContract, MockContract>
        {
            private readonly IInvocationMiddlewareLifeCycle _lifeCycle;

            public SampleInvocationMiddleware(IInvocationMiddlewareLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(IInvocationContext<MockContract, MockContract> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
            {
                await _lifeCycle.InnerEntersHandleAsync(context, next, cancellationToken);
                await base.HandleAsync(context, next, cancellationToken);
            }

            protected override async Task OnBeforeNextAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken)
            {
                await _lifeCycle.InnerEntersOnBeforeNextAsync(context, cancellationToken);
                await base.OnBeforeNextAsync(context, cancellationToken);
            }

            protected override async Task OnAfterNextAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken)
            {
                await _lifeCycle.InnerEntersOnAfterNextAsync(context, cancellationToken);
                await base.OnAfterNextAsync(context, cancellationToken);
            }

            protected override async Task OnErrorAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken)
            {
                await _lifeCycle.InnerEntersOnErrorAsync(context, cancellationToken);
                await base.OnErrorAsync(context, cancellationToken);
            }

            protected override async Task OnSucessAsync(IInvocationContext<MockContract, MockContract> context, CancellationToken cancellationToken)
            {
                await _lifeCycle.InnerEntersOnSucessAsync(context, cancellationToken);
                await base.OnSucessAsync(context, cancellationToken);
            }
        }
    }
}
