using SampleConsole.TennisGame.Contracts;
using Vesture.Mediator.Invocation;

namespace SampleConsole.TennisGame
{
    internal class TennisRallyMiddleware : InvocationMiddleware<TennisBallHit, TennisBallHitResult>
    {
        public override async Task HandleAsync(IInvocationContext<TennisBallHit, TennisBallHitResult> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            System.Console.WriteLine(context.Request);
            await base.HandleAsync(context, next, cancellationToken);
            System.Console.WriteLine(context.Response);
        }
    }
}
