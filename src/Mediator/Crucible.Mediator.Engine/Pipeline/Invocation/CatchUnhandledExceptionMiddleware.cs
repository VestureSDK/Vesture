using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation
{
    public class CatchUnhandledExceptionMiddleware : IPreInvocationPipelineMiddleware, IPreHandlerMiddleware
    {
        public static readonly CatchUnhandledExceptionMiddleware Instance = new();

        public async Task HandleAsync(IInvocationContext<object, object> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            try
            {
                await next.Invoke(cancellationToken);
            }
            catch (Exception ex)
            {
                context.SetError(ex);
            }
        }
    }
}
