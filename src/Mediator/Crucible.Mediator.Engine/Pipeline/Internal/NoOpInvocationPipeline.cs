using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    public class NoOpInvocationPipeline<TResponse> : IInvocationPipeline<TResponse>
    {
        private readonly IInvocationContextFactory _contextFactory;
        private readonly Action<IInvocationContext<TResponse>>? _contextSetup;

        public Type RequestType => typeof(object);

        public Type ResponseType => typeof(TResponse);

        public NoOpInvocationPipeline(IInvocationContextFactory contextFactory, Action<IInvocationContext<TResponse>>? contextSetup = null)
        {
            _contextFactory = contextFactory;
            _contextSetup = contextSetup;
        }

        public Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken)
        {
            var context = _contextFactory.CreateContextForRequest<object, TResponse>(request);
            _contextSetup?.Invoke(context);
            return Task.FromResult<IInvocationContext<TResponse>>(context);
        }
    }
}
