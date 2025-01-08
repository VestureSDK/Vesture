using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IMediator"/> in the <see cref="IServiceCollection"/> instance.
    /// </summary>
    public class RootMediatorBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="RootMediatorBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        public RootMediatorBuilder(IDependencyInjectionRegistrar registrar)
        {
            Registrar = registrar;
        }

        protected IDependencyInjectionRegistrar Registrar { get; }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="SingleHandlerPipelineBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public SingleHandlerPipelineBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new SingleHandlerPipelineBuilder<TRequest, TResponse>(Registrar);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="ICommand"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="ICommand"/> type.</typeparam>
        /// <returns>The <see cref="SingleHandlerPipelineBuilder{TRequest, CommandResponse}"/> for chaining.</returns>
        public SingleHandlerPipelineBuilder<TRequest, CommandResponse> Command<TRequest>()
        {
            return Request<TRequest, CommandResponse>();
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IEvent"/> type.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
        /// <returns>The <see cref="MultiHandlerPipelineBuilder{TEvent}"/> for chaining.</returns>
        public MultiHandlerPipelineBuilder<TEvent> Event<TEvent>()
        {
            return new MultiHandlerPipelineBuilder<TEvent>(Registrar);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="SingleHandlerPipelineBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public NoHandlerPipelineBuilder<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new NoHandlerPipelineBuilder<TRequest, TResponse>(Registrar);
        }
    }
}
