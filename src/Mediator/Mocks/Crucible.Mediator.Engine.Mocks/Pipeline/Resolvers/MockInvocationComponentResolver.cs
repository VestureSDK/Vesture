using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Moq;

namespace Crucible.Mediator.Engine.Mocks.Pipeline.Resolvers
{
    public class MockInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        public Mock<IInvocationComponentResolver<TComponent>> Mock { get; } = new Mock<IInvocationComponentResolver<TComponent>>();

        private IInvocationComponentResolver<TComponent> _inner => Mock.Object;

        public TComponent Component { get; set; } = default!;

        public MockInvocationComponentResolver()
        {
            Mock.Setup(m => m.ResolveComponent()).Returns(() => Component);
        }

        public TComponent ResolveComponent() => _inner.ResolveComponent();
    }
}
