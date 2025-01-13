using Crucible.Mediator.Abstractions.Tests.Bases;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Mocks;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests
{
    [MockTest]
    public class MockMediatorTest : MediatorConformanceTestBase<MockMediator>
    {
        private ICollection<Action<MockMediator>> _setups = [];

        protected override MockMediator CreateMediator()
        {
            var mediator = new MockMediator();

            foreach (var setup in _setups)
            {
                setup.Invoke(mediator);
            }

            return mediator;
        }

        protected override void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            _setups.Add(m => m.AddHandler(handler));
        }
        
        protected override void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            _setups.Add(m => m.AddMiddleware(middleware));
        }
    }
}
