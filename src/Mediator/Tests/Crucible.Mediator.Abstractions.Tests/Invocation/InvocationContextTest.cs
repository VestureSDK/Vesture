using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public class InvocationContextTest<TInvocationContext>
        where TInvocationContext : class, IInvocationContext
    {
        [Test]
        public void HasError_IsTrue_WhenErrorIsNotNull()
        {

        }
    }
}
