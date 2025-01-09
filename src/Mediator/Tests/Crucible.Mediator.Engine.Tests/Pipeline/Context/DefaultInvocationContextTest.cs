using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class DefaultInvocationContextTest_Event
        : InvocationContextTestBase_Event<object, DefaultInvocationContext<object, EventResponse>>
    {
        public DefaultInvocationContextTest_Event()
            : base(new object()) { }

        protected override DefaultInvocationContext<object, EventResponse> CreateInvocationContext(object request) => new(Request);

    }

    public class DefaultInvocationContextTest_Command
        : InvocationContextTestBase_Command<object, DefaultInvocationContext<object, CommandResponse>>
    {
        public DefaultInvocationContextTest_Command()
            : base(new object()) { }

        protected override DefaultInvocationContext<object, CommandResponse> CreateInvocationContext(object request) => new(Request);

    }

    public class DefaultInvocationContextTest_Request 
        : InvocationContextTestBase_Request<object, object, DefaultInvocationContext<object, object>>
    {
        public DefaultInvocationContextTest_Request()
        : base(new object()) { }

        protected override DefaultInvocationContext<object, object> CreateInvocationContext(object request) => new(Request);
        
        protected override object CreateResponse() => new object();
    }
}
