namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines a base invoker.
    /// </summary>
    public abstract class Invoker
    {
        private readonly IInvocationPipelineProvider _pipelineProvider;

        /// <summary>
        /// Initializes a new <see cref="Invoker"/> instance.
        /// </summary>
        /// <param name="pipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        protected Invoker(IInvocationPipelineProvider pipelineProvider)
        {
            _pipelineProvider = pipelineProvider;
        }

        /// <inheritdoc/>
        public async Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var pipeline = _pipelineProvider.GetInvocationPipeline<TResponse>(request);
            var context = await pipeline.ExecuteAndCaptureAsync(request, cancellationToken);

            return context;
        }

        /// <inheritdoc/>
        public async Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var pipeline = _pipelineProvider.GetInvocationPipeline<TResponse>(request);
            var context = await pipeline.ExecuteAndCaptureAsync(request, cancellationToken);

            return ThrowIfContextHasErrorOrReturnResponse(context);
        }

        /// <summary>
        /// <c>throw</c> the <see cref="IInvocationContext.Error"/> if the specified <paramref name="context"/> has <see cref="IInvocationContext.HasError"/>.
        /// </summary>
        /// <param name="context">The <see cref="IInvocationContext"/> instance.</param>
        protected static void ThrowIfContextHasError(IInvocationContext context)
        {
            if (context.HasError)
            {
                // If an error occured, then throw it.
                throw context.Error!;
            }
        }

        /// <summary>
        /// <c>throw</c> the <see cref="IInvocationContext.Error"/> if the specified <paramref name="context"/> has <see cref="IInvocationContext.HasError"/>.
        /// Else returns the specified <paramref name="context"/> <see cref="IInvocationContext.Response"/> or <c>default</c> if not available.
        /// </summary>
        /// <typeparam name="TResponse">The response type as present in the <paramref name="context"/>.</typeparam>
        /// <param name="context">The <see cref="IInvocationContext{TResponse}"/> instance.</param>
        /// <returns>The response as present in the <paramref name="context"/> or <c>default</c>.</returns>
        protected static TResponse ThrowIfContextHasErrorOrReturnResponse<TResponse>(IInvocationContext<TResponse> context)
        {
            ThrowIfContextHasError(context);

#pragma warning disable CS8603 // Possible null reference return.
            // If the context is successful, then return the response
            return context.HasResponse ? context.Response! : default;
#pragma warning restore CS8603 // Possible null reference return.
        }

    }
}
