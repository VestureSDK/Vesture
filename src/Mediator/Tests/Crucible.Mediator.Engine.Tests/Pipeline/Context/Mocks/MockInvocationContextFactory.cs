using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks
{
    public class MockInvocationContextFactory : MockInvocationContextFactory<MockContract, MockContract>
    {

    }

    public class MockInvocationContextFactory<TContextRequest, TContextResponse> : IInvocationContextFactory
    {
        public Mock<IInvocationContextFactory> Mock { get; } = new();

        private IInvocationContextFactory _inner => Mock.Object;

        public MockInvocationContextFactory()
            : this(new MockInvocationContext<TContextRequest, TContextResponse>()) { }

        public MockInvocationContextFactory(IInvocationContext<TContextRequest, TContextResponse> context)
        {
            Mock.Setup(m => m.CreateContextForRequest<TContextRequest, TContextResponse>(It.IsAny<TContextRequest>()!))
                .Returns(context);
        }

        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request) => _inner.CreateContextForRequest<TRequest, TResponse>(request);
    }
}
