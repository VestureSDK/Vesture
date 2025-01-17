using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Requests;

namespace Ingot.Mediator.DependencyInjection.Fluent
{
    /// <summary>
    /// <para>
    /// The <see cref="RootFluentMediatorComponentRegistrar"/>
    /// is a builder to configure the <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// It is the root of the fluent mediator registration builders.
    /// </para>
    /// </summary>
    /// <seealso cref="IMediatorComponentRegistrar"/>
    /// <seealso cref="NoHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// <seealso cref="MultiHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    /// <seealso cref="SingleHandlerFluentMediatorComponentRegistrar{TRequest, TResponse}"/>
    public class RootFluentMediatorComponentRegistrar
    {
        /// <summary>
        /// Initializes a new <see cref="RootFluentMediatorComponentRegistrar"/> instance.
        /// </summary>
        /// <param name="registrar">The <see cref="IMediatorComponentRegistrar"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="registrar"/> is <see langword="null" />.</exception>
        public RootFluentMediatorComponentRegistrar(IMediatorComponentRegistrar registrar)
        {
            ArgumentNullException.ThrowIfNull(registrar, nameof(registrar));

            Registrar = registrar;
        }

        /// <summary>
        /// The <see cref="IMediatorComponentRegistrar"/> instance.
        /// </summary>
        protected IMediatorComponentRegistrar Registrar { get; }

        /// <summary>
        /// Initializes a flow to register an <see cref="IInvocationPipeline{TResponse}"/>
        /// component related to a <see cref="IRequest{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type handled by this pipeline.</typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public SingleHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new SingleHandlerFluentMediatorComponentRegistrar<TRequest, TResponse>(Registrar);
        }

        /// <summary>
        /// Initializes a flow to register an <see cref="IInvocationPipeline{TResponse}"/>
        /// component related to a <see cref="ICommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">The <see cref="ICommand"/> type handled by this pipeline.</typeparam>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public SingleHandlerFluentMediatorComponentRegistrar<TCommand, CommandResponse> Command<TCommand>()
        {
            return Request<TCommand, CommandResponse>();
        }

        /// <summary>
        /// Initializes a flow to register an <see cref="IInvocationPipeline{TResponse}"/>
        /// component related to a <see cref="IEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEvent"/> type handled by this pipeline.</typeparam>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public MultiHandlerFluentMediatorComponentRegistrar<TEvent, EventResponse> Event<TEvent>()
        {
            return new MultiHandlerFluentMediatorComponentRegistrar<TEvent, EventResponse>(Registrar);
        }

        /// <summary>
        /// Initializes a flow to register <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
        /// for an <see cref="IInvocationPipeline{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TRequest">The contract type handled by this pipeline.</typeparam>
        /// <typeparam name="TResponse"><inheritdoc cref="IInvocationHandler{TRequest, TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
        /// <returns>The next fluent registrar instance to pursue registration.</returns>
        public NoHandlerFluentMediatorComponentRegistrar<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new NoHandlerFluentMediatorComponentRegistrar<TRequest, TResponse>(Registrar);
        }
    }
}
