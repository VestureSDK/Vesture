using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.DependencyInjection.MSDI;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// An <see cref="IMediatorComponentRegistrar"/> allows to register
    /// <see cref="IInvocationPipeline{TResult}"/> components 
    /// in an Inversion Of Control container.
    /// </summary>
    /// <seealso cref="MSDIMediatorComponentRegistrar"/>
    public interface IMediatorComponentRegistrar
    {
        /// <summary>
        /// Registers an <see cref="IInvocationHandler{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TRequest']"/></typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <typeparam name="THandler">The <see cref="IInvocationHandler{TRequest, TResponse}"/> type to register.</typeparam>
        /// <param name="singleton">Defines if the Inversion Of Control container should treat the registered as a singleton or transient.</param>
        void RegisterHandler<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(bool singleton = false)
            where THandler : class, IInvocationHandler<TRequest, TResponse>;

        /// <summary>
        /// Registers an <see cref="IInvocationHandler{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TRequest']"/></typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <param name="handler">The singleton <see cref="IInvocationHandler{TRequest, TResponse}"/> instance.</param>
        void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler);

        /// <summary>
        /// Registers an <see cref="IInvocationHandler{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest">The contract type handled by the underlying handler(s).</typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <typeparam name="TStrategy">The <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> type to register.</typeparam>
        void RegisterHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>;

        /// <summary>
        /// Registers an <see cref="IInvocationHandler{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest">The contract type handled by hte underlying handler(s).</typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <param name="strategy">The singleton <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> instance.</param>
        void RegisterHandlerStrategy<TRequest, TResponse>(IInvocationHandlerStrategy<TRequest, TResponse> strategy);

        /// <summary>
        /// Registers an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest"><inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> type to register.</typeparam>
        /// <param name="order"><inheritdoc cref="IMiddlewareInvocationPipelineItem.Order" path="/summary"/></param>
        /// <param name="singleton">Defines if the Inversion Of Control container should treat the registered as a singleton or transient.</param>
        void RegisterMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>;

        /// <summary>
        /// Registers an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> in the Inversion Of Control container.
        /// </summary>
        /// <typeparam name="TRequest"><inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <param name="middleware">The singleton <see cref="IInvocationMiddleware{TRequest, TResponse}"/> instance.</param>
        /// <param name="order"><inheritdoc cref="IMiddlewareInvocationPipelineItem.Order" path="/summary"/></param>
        void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware, int? order = null);
    }
}
