using System.Diagnostics.CodeAnalysis;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.DependencyInjection.Fluent
{
    /// <summary>
    /// The <see cref="SingleHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// is a builder to register <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// and <see cref="IInvocationHandler{TRequest, TResponse}"/>
    /// for an <see cref="IInvocationPipeline{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The contract type handled by this pipeline.</typeparam>
    /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
    /// <seealso cref="IMediatorComponentRegistrar"/>
    /// <seealso cref="RootFluentMediatorComponentRegistrar"/>
    /// <seealso cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// <seealso cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    public class SingleHandlerFluentMediatorComponentRegistrar<TRequest, TResponse>
        : RootFluentMediatorComponentRegistrar
    {
        /// <summary>
        /// Initializes a new <see cref="SingleHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="registrar">The <see cref="IMediatorComponentRegistrar"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="registrar"/> is <see langword="null" />.</exception>
        public SingleHandlerFluentMediatorComponentRegistrar(IMediatorComponentRegistrar registrar)
            : base(registrar) { }

        /// <inheritdoc cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.AddMiddleware{TMiddleware}(int?, bool)"/>
        public SingleHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> AddMiddleware<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TMiddleware
        >(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware<TRequest, TResponse, TMiddleware>(order, singleton);
            return this;
        }

        /// <inheritdoc cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.AddMiddleware{TMiddleware}(TMiddleware, int?)"/>
        public SingleHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> AddMiddleware(
            IInvocationMiddleware<TRequest, TResponse> middleware,
            int? order = null
        )
        {
            Registrar.RegisterMiddleware(middleware, order);
            return this;
        }

        /// <inheritdoc cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.HandleWith{THandler}(bool)"/>
        public RootFluentMediatorComponentRegistrar HandleWith<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler
        >(bool singleton = false)
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            Registrar.RegisterHandler<TRequest, TResponse, THandler>(singleton);
            return this;
        }

        /// <inheritdoc cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.HandleWith(IInvocationHandler{TRequest,TResponse})"/>
        public RootFluentMediatorComponentRegistrar HandleWith(
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            Registrar.RegisterHandler(handler);
            return this;
        }
    }
}
