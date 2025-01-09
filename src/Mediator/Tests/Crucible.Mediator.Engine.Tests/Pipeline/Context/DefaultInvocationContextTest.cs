using Crucible.Mediator.Abstractions.Tests.Commands;
using Crucible.Mediator.Abstractions.Tests.Events;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Requests;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class DefaultInvocationContextTest_Event
        : InvocationContextTestBase_Event<MockEvent, DefaultInvocationContext<MockEvent, EventResponse>>
    {
        public DefaultInvocationContextTest_Event()
            : base(new ()) { }

        protected override DefaultInvocationContext<MockEvent, EventResponse> CreateInvocationContext(MockEvent @event) => new(@event);

    }

    public class DefaultInvocationContextTest_Command
        : InvocationContextTestBase_Command<MockCommand, DefaultInvocationContext<MockCommand, CommandResponse>>
    {
        public DefaultInvocationContextTest_Command()
            : base(new ()) { }

        protected override DefaultInvocationContext<MockCommand, CommandResponse> CreateInvocationContext(MockCommand command) => new(command);

    }

    public class DefaultInvocationContextTest_Request 
        : InvocationContextTestBase_Request<MockRequest, MockResponse, DefaultInvocationContext<MockRequest, MockResponse>>
    {
        public DefaultInvocationContextTest_Request()
        : base(new ()) { }

        protected override DefaultInvocationContext<MockRequest, MockResponse> CreateInvocationContext(MockRequest request) => new(request);
        
        protected override MockResponse CreateResponse() => new ();
    }
}
