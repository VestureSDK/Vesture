using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Moq;
using Any = object;

namespace Ingot.Mediator.Engine.Mocks.Pipeline
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
