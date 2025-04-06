using Moq;

namespace Vesture.Mediator.Mocks.Invocation
{
    /// <summary>
    /// Defines a mock "next" func as expected by a middleware.
    /// </summary>
    public class MockNext
    {
        private Task _returnTask = Task.CompletedTask;

        /// <summary>
        /// The <see cref="Mock{T}"/> instance.
        /// </summary>
        public Mock<Func<CancellationToken, Task>> Mock { get; } = new();

        /// <summary>
        /// The func delegate as setup by the Mock.
        /// </summary>
        public Func<CancellationToken, Task> Delegate => Mock.Object;

        /// <summary>
        /// The delegate task if not overriden by a mock setup.
        /// </summary>
        public Task ReturnTask
        {
            get => _returnTask;
            set => _returnTask = value ?? Task.CompletedTask;
        }

        /// <summary>
        /// Initializes a new <see cref="MockNext"/> instance.
        /// </summary>
        public MockNext()
        {
            Mock.Setup(m => m(It.IsAny<CancellationToken>())).Returns(() => ReturnTask);
        }

        /// <summary>
        /// Converts the mock to its delegate.
        /// </summary>
        /// <param name="mock">The mock instance.</param>
        public static implicit operator Func<CancellationToken, Task>(MockNext mock)
        {
            return mock.Delegate;
        }
    }
}
