using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks
{
    public class MockInvocationContextFactory : IInvocationContextFactory
    {
        private Action<object>? _contextFactorySetup;

        public void SetupCreateContextFactory<TRequest, TResponse>(Action<MockInvocationContextFactory<TRequest, TResponse>>? contextFactorySetup)
        {
            _contextFactorySetup = (context) => contextFactorySetup?.Invoke((MockInvocationContextFactory<TRequest, TResponse>)context);
        }

        public IInvocationContextFactory CreateContextFactory<TRequest, TResponse>(object request)
        {
            var contextFactory = new MockInvocationContextFactory<TRequest, TResponse>
            {
                Request = (TRequest)request
            };

            _contextFactorySetup?.Invoke(contextFactory);

            return contextFactory;
        }

        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request)
        {
            var contextFactory = CreateContextFactory<TRequest, TResponse>(request);
            return contextFactory.CreateContextForRequest<TRequest, TResponse>(request);
        }
    }

    public class MockInvocationContextFactory<TContextRequest, TContextResponse> : IInvocationContextFactory
    {
        public Mock<IInvocationContextFactory> Mock { get; } = new();

        private IInvocationContextFactory _inner => Mock.Object;

        private TContextRequest _request;

        public TContextRequest Request 
        { 
            get => _request;
            set
            {
                _request = value;
                _managedContext.Request = value!;
            }
        }

        private readonly MockInvocationContext<TContextRequest, TContextResponse> _managedContext;

        private IInvocationContext<TContextRequest, TContextResponse>? _context;

        public IInvocationContext<TContextRequest, TContextResponse> Context
        {
            get => _context ?? _managedContext;
            set => _context = value;
        }

        public MockInvocationContextFactory()
        {
            _request = default!;
            _managedContext = new MockInvocationContext<TContextRequest, TContextResponse>
            {
                Request = _request
            };

            Mock.Setup(m => m.CreateContextForRequest<TContextRequest, TContextResponse>(It.IsAny<TContextRequest>()!))
                .Returns(() => Context);
        }

        public IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(object request) => _inner.CreateContextForRequest<TRequest, TResponse>(request);
    }
}
