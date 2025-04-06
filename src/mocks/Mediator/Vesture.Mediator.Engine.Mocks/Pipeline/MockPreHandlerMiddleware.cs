using Moq;
using Vesture.Mediator.Engine.Pipeline;
using Vesture.Mediator.Invocation;
using Vesture.Mediator.Mocks.Invocation;
using Any = object;

namespace Vesture.Mediator.Engine.Mocks.Pipeline
{
    public class MockPreHandlerMiddleware
        : MockInvocationMiddleware<Any, Any>,
            IPreHandlerMiddleware
    {
        public MockPreHandlerMiddleware()
        {
            Mock.Setup(m =>
                    m.HandleAsync(
                        It.IsAny<IInvocationContext<Any, Any>>(),
                        It.IsAny<Func<CancellationToken, Task>>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns<
                    IInvocationContext<Any, Any>,
                    Func<CancellationToken, Task>,
                    CancellationToken
                >(
                    async (context, next, cancellationtoken) =>
                    {
                        try
                        {
                            await next(cancellationtoken);
                        }
                        catch (Exception ex)
                        {
                            context.AddError(ex);
                        }
                    }
                );
        }
    }
}
