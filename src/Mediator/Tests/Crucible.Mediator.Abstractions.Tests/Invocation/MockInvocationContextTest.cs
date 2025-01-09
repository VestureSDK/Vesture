using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public class MockInvocationContextTest_Event :
        InvocationContextTestBase_Event<object, MockInvocationContext<object, EventResponse>>
    {
        public MockInvocationContextTest_Event()
            : base(new()) { }

        protected override MockInvocationContext<object, EventResponse> CreateInvocationContext(object request) => new(Request);
    }

    public class MockInvocationContextTest_Command :
        InvocationContextTestBase_Command<object, MockInvocationContext<object, CommandResponse>>
    {
        public MockInvocationContextTest_Command()
            : base(new()) { }

        protected override MockInvocationContext<object, CommandResponse> CreateInvocationContext(object request) => new(Request);
    }

    public class MockInvocationContextTest_Request :
        InvocationContextTestBase_Request<object, object, MockInvocationContext<object, object>>
    {
        public MockInvocationContextTest_Request()
            : base(new()) { }

        protected override object CreateResponse() => new();

        protected override MockInvocationContext<object, object> CreateInvocationContext(object request) => new(Request);
    }
}
