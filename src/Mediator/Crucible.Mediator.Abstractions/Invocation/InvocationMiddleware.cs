using System.Diagnostics;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Base implementation of <see cref="IInvocationMiddleware{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> or <see cref="ICommand"/> type.</typeparam>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/> or 
    /// <see cref="NoResponse"/> for an <see cref="ICommand"/> or <see cref="IEvent"/>.</typeparam>
    [DebuggerDisplay("{typeof(TRequest).Name} -> {typeof(TResponse).Name}")]
    public abstract class InvocationMiddleware<TRequest, TResponse> : IInvocationMiddleware<TRequest, TResponse>
    {
        /// <inheritdoc/>
        /// <remarks>
        /// By default it will do the following:
        /// <list type="number">
        /// <item>Call <see cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>. Override <see cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/> to alter the behavior.</item>
        /// <item>Then call <paramref name="next"/></item>
        /// <item>And finally <see cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>. Override <see cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/> to alter the behavior.</item>
        /// </list>
        /// </remarks>
        public virtual async Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            await OnBeforeNextAsync(context, cancellationToken).ConfigureAwait(false);

            await next.Invoke(cancellationToken).ConfigureAwait(false);

            await OnAfterNextAsync(context, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/> before calling the next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or handler.
        /// </summary>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected virtual Task OnBeforeNextAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/> after calling the next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or handler.
        /// </summary>
        /// <remarks>
        /// By default, it checks if <see cref="IInvocationContext.HasError"/> and invokes <see cref="OnErrorAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>
        /// else it invokes <see cref="OnSucessAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>.
        /// </remarks>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected virtual Task OnAfterNextAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            if (context.HasError)
            {
                return OnErrorAsync(context, cancellationToken);
            }
            else
            {
                return OnSucessAsync(context, cancellationToken);
            }
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/> after calling the next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or handler and an <see cref="Exception"/> occured.
        /// </summary>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected virtual Task OnErrorAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/> after calling the next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or handler and it is successful.
        /// </summary>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        protected virtual Task OnSucessAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
