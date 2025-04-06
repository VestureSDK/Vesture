using System.Diagnostics.CodeAnalysis;
using Vesture.Mediator.Engine.Pipeline;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.DependencyInjection.Fluent
{
    /// <summary>
    /// The <see cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// is a builder to register <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// for an <see cref="IInvocationPipeline{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The contract type handled by this pipeline.</typeparam>
    /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
    /// <seealso cref="IMediatorComponentRegistrar"/>
    /// <seealso cref="RootFluentMediatorComponentRegistrar"/>
    /// <seealso cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// <seealso cref="SingleHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    public class NoHandlerFluentMediatorComponentRegistrar<TRequest, TResponse>
        : RootFluentMediatorComponentRegistrar
    {
        /// <summary>
        /// Initializes a new <see cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="registrar">The <see cref="IMediatorComponentRegistrar"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="registrar"/> is <see langword="null" />.</exception>
        public NoHandlerFluentMediatorComponentRegistrar(IMediatorComponentRegistrar registrar)
            : base(registrar) { }

        /// <summary>
        /// Adds the <typeparamref name="TMiddleware"/> to the <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> type to register.</typeparam>
        /// <param name="order"><inheritdoc cref="IMediatorComponentRegistrar.RegisterMiddleware{TRequest, TResponse, TMiddleware}(int?, bool)" path="/param[@name='order']"/></param>
        /// <param name="singleton"><inheritdoc cref="IMediatorComponentRegistrar.RegisterMiddleware{TRequest, TResponse, TMiddleware}(int?, bool)" path="/param[@name='singleton']"/></param>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public NoHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> AddMiddleware<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TMiddleware
        >(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware<TRequest, TResponse, TMiddleware>(order, singleton);
            return this;
        }

        /// <param name="middleware">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> singleton instance.</param>
        /// <param name="order"><inheritdoc cref="AddMiddleware{TMiddleware}(int?, bool)" path="/param[@name='order']"/></param>
        /// <inheritdoc cref="AddMiddleware{TMiddleware}(int?, bool)"/>
        public NoHandlerFluentMediatorComponentRegistrar<
            TRequest,
            TResponse
        > AddMiddleware<TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware(middleware, order);
            return this;
        }
    }
}
