namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines a base generic handler with helper methods.
    /// </summary>
    public abstract class InvocationHandler
    {
        /// <summary>
        /// Wraps the execution of <paramref name="action"/> within a <c>try { } catch</c> to ensure the <paramref name="context"/>
        /// contains any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="context">The <see cref="IInvocationContext"/> instance to pass to the <paramref name="action"/>.</param>
        /// <param name="action">The action to execute against the <paramref name="context"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to pass to the <paramref name="action"/>.</param>
        /// <returns>The executing <paramref name="action"/>.</returns>
        protected static async Task WrapExecutionAsync(IInvocationContext context, Func<CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await action.Invoke(cancellationToken);
            }
            catch (Exception ex)
            {
                context.SetError(ex);
            }
        }

        /// <inheritdoc cref="WrapExecutionAsync(IInvocationContext, Func{CancellationToken, Task}, CancellationToken)" />
        /// <typeparam name="TResponse">The expected response type when handling the <paramref name="context"/>.</typeparam>
        protected static Task WrapExecutionAsync<TResponse>(IInvocationContext context, Func<CancellationToken, Task<TResponse>> action, CancellationToken cancellationToken)
        {
            return WrapExecutionAsync(context, async (ct) =>
            {
                var response = await action.Invoke(ct);
                context.SetResponse(response!);
            }, cancellationToken);
        }
    }

    /// <summary>
    /// Base implementation of an invocation handler.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    public abstract class NoResponseInvocationHandler<TRequest, TNoResponse> : InvocationHandler
        where TNoResponse: NoResponse
    {
        /// <inheritdoc/>
        /// <remarks>
        /// Internally calls <see cref="ExecuteAsync(TRequest, CancellationToken)"/> after checking if <paramref name="cancellationToken"/>
        /// has been cancelled already or not by calling <see cref="CancellationToken.ThrowIfCancellationRequested"/>.
        /// At the end, <see cref="IInvocationContext.Error"/> might be set if an <see cref="Exception"/> occured.
        /// </remarks>
        public virtual Task ExecuteAsync(IInvocationContext<TRequest, TNoResponse> context, CancellationToken cancellationToken = default)
        {
            return WrapExecutionAsync(context, (ct) => ExecuteAsync(context.Request, ct), cancellationToken);
        }

        /// <summary>
        /// Executes the handling process for the related <typeparamref name="TRequest"/>.
        /// </summary>
        /// <remarks>
        /// This is the internal implementation of <see cref="ExecuteAsync(IInvocationContext{TRequest, TNoResponse}, CancellationToken)"/>.
        /// It is implicitely surrounded by <c>try { } catch</c> and can safely <c>throw</c>.
        /// </remarks>
        /// <param name="request">The <typeparamref name="TRequest"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }

    /// <inheritdoc cref="NoResponseInvocationHandler{TRequest, TNoResponse}"/>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    public abstract class InvocationHandler<TRequest, TResponse> : InvocationHandler
    {
        /// <inheritdoc/>
        /// <remarks>
        /// Internally calls <see cref="ExecuteAsync(TRequest, CancellationToken)"/> after checking if <paramref name="cancellationToken"/>
        /// has been cancelled already or not by calling <see cref="CancellationToken.ThrowIfCancellationRequested"/>.
        /// At the end, either <see cref="IInvocationContext.Error"/> or <see cref="IInvocationContext.Response"/> is set.
        /// </remarks>
        public virtual Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            return WrapExecutionAsync(context, (ct) => ExecuteAsync(context.Request, ct), cancellationToken);
        }

        /// <summary>
        /// Executes the handling process for the related <typeparamref name="TRequest"/>.
        /// </summary>
        /// <remarks>
        /// This is the internal implementation of <see cref="ExecuteAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>.
        /// It is implicitely surrounded by <c>try { } catch</c> and can safely <c>throw</c>.
        /// </remarks>
        /// <param name="request">The <typeparamref name="TRequest"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }
}
