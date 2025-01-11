using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockNext
    {
        private Task _returnTask = Task.CompletedTask;

        public Mock<Func<CancellationToken, Task>> Mock { get; } = new ();

        public Func<CancellationToken, Task> Delegate => Mock.Object;

        public Task ReturnTask 
        { 
            get => _returnTask; 
            set => _returnTask = value ?? Task.CompletedTask; 
        }

        public MockNext()
        {
            Mock
                .Setup(m => m(It.IsAny<CancellationToken>()))
                .Returns(() => ReturnTask);
        }

        public static implicit operator Func<CancellationToken, Task>(MockNext mock)
        {
            return mock.Delegate;
        }
    }
}
