using System.Diagnostics;
using Vesture.Mediator.Commands;
using Vesture.Mediator.Events;
using Vesture.Mediator.Requests;

namespace Vesture.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// The <see cref="InvocationMiddleware{TRequest, TResponse}"/> provides a base implementation of <see cref="IInvocationMiddleware{TRequest, TResponse}"/>.
    /// This class defines the common structure for middleware that can be used in the request,
    /// command, or event processing pipeline in the context of the mediator pattern.
    /// </para>
    /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}" path="/summary"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This base class allows you to define custom behavior by overriding the methods:
    /// <list type="bullet">
    /// <item>
    /// <term><see cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/></term>
    /// <description>for logic to run before the next middleware or handler is invoked.</description>
    /// </item>
    /// <item>
    /// <term><see cref="OnSucessAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/></term>
    /// <description>for logic to run on a successful next middleware or handler invocation.</description>
    /// </item>
    /// <item>
    /// <term><see cref="OnErrorAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/></term>
    /// <description>for logic to run when the next middleware or handler encountered an error.</description>
    /// </item>
    /// <item>
    /// <term><see cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/></term>
    /// <description>for logic to run after the next middleware or handler invocation has completed.
    /// This replaces the <c>OnSucessAsync</c> and <c>OnErrorAsync</c> overrides.</description>
    /// </item>
    /// <item>
    /// <term><see cref="HandleAsync(IInvocationContext{TRequest, TResponse}, Func{CancellationToken, Task}, CancellationToken)"/></term>
    /// <description>for fully custom logic. This replaces all the other overrides.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <typeparam name="TRequest">
    /// The type of the request, command, or event being processed by the middleware.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response produced by the handler processing the <typeparamref name="TRequest"/>.
    /// </typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    [DebuggerDisplay("{typeof(TRequest).Name} -> {typeof(TResponse).Name}")]
    public abstract class InvocationMiddleware<TRequest, TResponse>
        : IInvocationMiddleware<TRequest, TResponse>
    {
        /// <inheritdoc/>
        /// <remarks>
        /// By default, the execution sequence will:
        /// <list type="number">
        /// <item>Call <see cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/> before invoking the next middleware or handler.</item>
        /// <item>Invoke the next middleware or handler by calling <paramref name="next"/>.</item>
        /// <item>Finally, call <see cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/> after the next middleware or handler has completed.</item>
        /// </list>
        /// You can override the methods <see cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>
        /// and <see cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/> to customize
        /// the behavior for your specific use case.
        /// </remarks>
        public virtual async Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        )
        {
            await OnBeforeNextAsync(context, cancellationToken).ConfigureAwait(false);

            await next.Invoke(cancellationToken).ConfigureAwait(false);

            await OnAfterNextAsync(context, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/>
        /// before calling the next middleware or handler.
        /// </summary>
        /// <param name="context">
        /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}.HandleAsync(IInvocationContext{TRequest, TResponse}, Func{CancellationToken, Task}, CancellationToken)" path="/param[@name='context']"/>
        /// </param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}.HandleAsync(IInvocationContext{TRequest, TResponse}, Func{CancellationToken, Task}, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task OnBeforeNextAsync(
            IInvocationContext<TRequest, TResponse> context,
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/>
        /// after calling the next middleware or handler.
        /// </summary>
        /// <remarks>
        /// By default, this method checks if the <see cref="IInvocationContext{TRequest, TResponse}"/> contains errors
        /// (<see cref="IInvocationContext.HasError"/>).
        /// <list type="bullet">
        /// <item>
        /// If errors are present, it will call <see cref="OnErrorAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>.
        /// </item>
        /// <item>
        /// Otherwise, it will call <see cref="OnSucessAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>.
        /// </item>
        /// </list>
        /// </remarks>
        /// <inheritdoc cref="OnBeforeNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>
        protected virtual Task OnAfterNextAsync(
            IInvocationContext<TRequest, TResponse> context,
            CancellationToken cancellationToken
        )
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
        /// Executes the middleware process for the related <paramref name="context"/> after calling the next
        /// middleware or handler and an error has occurred.
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>
        protected virtual Task OnErrorAsync(
            IInvocationContext<TRequest, TResponse> context,
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/> after calling the next
        /// middleware or handler and the operation was successful.
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc cref="OnAfterNextAsync(IInvocationContext{TRequest, TResponse}, CancellationToken)"/>
        protected virtual Task OnSucessAsync(
            IInvocationContext<TRequest, TResponse> context,
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }
    }
}
