using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Mocks.Invocation;
using Moq;
using Any = object;

namespace Crucible.Mediator.Engine.Mocks.Pipeline
{
    public class MockPreHandlerMiddleware : MockInvocationMiddleware<Any, Any>, IPreHandlerMiddleware
    {
        public MockPreHandlerMiddleware()
        {
            Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<Any, Any>, Func<CancellationToken, Task>, CancellationToken>(async (context, next, cancellationtoken) =>
                {
                    try
                    {
                        await next(cancellationtoken);
                    }
                    catch (Exception ex)
                    {
                        context.AddError(ex);
                    }
                });
        }
    }
}
