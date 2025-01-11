using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Moq;
using static Crucible.Mediator.Abstractions.Tests.Commands.CommandHandlerTest;

namespace Crucible.Mediator.Abstractions.Tests.Commands
{
    [SampleTest]
    public class CommandHandlerTest : InvocationHandlerConformanceTestBase<MockCommand, CommandResponse, SampleCommandHandler>
    {
        protected Mock<ICommandHandlerLifeCycle> LifeCycle { get; } = new();

        public CommandHandlerTest()
            : base(new()) { }

        protected override SampleCommandHandler CreateInvocationHandler() => new(LifeCycle.Object);

        [Test]
        public async Task HandleAsync_InnerInvokes()
        {
            // Arrange
            var entersHandleAsyncTaskCompletionSource = new TaskCompletionSource();
            LifeCycle.Setup(m => m.InnerEntersHandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()))
                .Returns(entersHandleAsyncTaskCompletionSource.Task);

            // Act / Assert
            var task = ((IInvocationHandler<MockCommand, CommandResponse>)Handler).HandleAsync(Request, CancellationToken);

            LifeCycle.Verify(m => m.InnerEntersHandleAsync(It.IsAny<MockCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            entersHandleAsyncTaskCompletionSource.SetResult();
            await task;
        }

        public interface ICommandHandlerLifeCycle
        {
            Task InnerEntersHandleAsync(MockCommand command, CancellationToken cancellationToken);
        }

        public class SampleCommandHandler : CommandHandler<MockCommand>
        {
            private readonly ICommandHandlerLifeCycle _lifeCycle;

            public SampleCommandHandler(ICommandHandlerLifeCycle lifeCycle)
            {
                _lifeCycle = lifeCycle;
            }

            public override async Task HandleAsync(MockCommand command, CancellationToken cancellationToken = default)
            {
                await _lifeCycle.InnerEntersHandleAsync(command, cancellationToken);
            }
        }
    }
}
