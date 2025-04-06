using System.Diagnostics.CodeAnalysis;
using Vesture.Mediator.Engine.Pipeline;
using Vesture.Mediator.Engine.Pipeline.Strategies;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.DependencyInjection.Fluent
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
    /// <seealso cref="SingleHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    public class MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse>
        : RootFluentMediatorComponentRegistrar
    {
        /// <summary>
        /// Initializes a new <see cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="registrar">The <see cref="IMediatorComponentRegistrar"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="registrar"/> is <see langword="null" />.</exception>
        public MultiHandlerFluentMediatorComponentRegistrar(IMediatorComponentRegistrar registrar)
            : base(registrar) { }

        /// <inheritdoc cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.AddMiddleware{TMiddleware}(int?, bool)"/>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> AddMiddleware<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TMiddleware
        >(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware<TRequest, TResponse, TMiddleware>(order, singleton);
            return this;
        }

        /// <inheritdoc cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}.AddMiddleware{TMiddleware}(TMiddleware, int?)"/>
        public MultiHandlerFluentMediatorComponentRegistrar<
            TRequest,
            TResponse
        > AddMiddleware<TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware(middleware, order);
            return this;
        }

        /// <summary>
        /// Adds the <typeparamref name="THandler"/> to the <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IInvocationHandler{TRequest, TResponse}"/> type to register.</typeparam>
        /// <param name="singleton"><inheritdoc cref="IMediatorComponentRegistrar.RegisterHandler{TRequest, TResponse, THandler}(bool)" path="/param[@name='singleton']"/></param>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> HandleWith<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler
        >(bool singleton = false)
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            Registrar.RegisterHandler<TRequest, TResponse, THandler>(singleton);
            return this;
        }

        /// <param name="handler">The <see cref="IInvocationHandler{TRequest, TResponse}"/> singleton instance.</param>
        /// <inheritdoc cref="HandleWith{THandler}(bool)"/>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> HandleWith(
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            Registrar.RegisterHandler(handler);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ParallelHandlersStrategy{TRequest, TResponse}"/> as
        /// <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> for the
        /// <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> InParallel()
        {
            return WithStrategy<ParallelHandlersStrategy<TRequest, TResponse>>();
        }

        /// <summary>
        /// Sets the <see cref="SequentialHandlersStrategy{TRequest, TResponse}"/> as
        /// <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> for the
        /// <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> InSequence()
        {
            return WithStrategy<SequentialHandlersStrategy<TRequest, TResponse>>();
        }

        /// <summary>
        /// Sets the <typeparamref name="TStrategy"/> to the <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> type to register.</typeparam>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public MultiHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> WithStrategy<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TStrategy
        >()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>
        {
            Registrar.RegisterHandlerStrategy<TRequest, TResponse, TStrategy>();
            return this;
        }
    }
}
