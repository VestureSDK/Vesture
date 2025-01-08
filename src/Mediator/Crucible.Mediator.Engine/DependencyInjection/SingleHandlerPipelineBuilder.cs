using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="ICommandHandler{TCommand}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class SingleHandlerPipelineBuilder<TRequest, TResponse> : RootMediatorBuilder
    {
        public SingleHandlerPipelineBuilder(IDependencyInjectionRegistrar registrar)
            : base(registrar)
        {
        }

        public SingleHandlerPipelineBuilder<TRequest, TResponse> AddMiddleware<TMiddleware>(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            Registrar.RegisterMiddleware<TRequest, TResponse, TMiddleware>(order, singleton);
            return this;
        }

        public SingleHandlerPipelineBuilder<TRequest, TResponse> AddMiddleware(IInvocationMiddleware<TRequest, TResponse> middleware, int? order = null)
        {
            Registrar.RegisterMiddleware<TRequest, TResponse>(middleware, order);
            return this;
        }

        public RootMediatorBuilder HandleWith<THandler>()
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            Registrar.RegisterHandler<TRequest, TResponse, THandler>();
            return this;
        }

        public RootMediatorBuilder HandleWith(IInvocationHandler<TRequest, TResponse> handler)
        {
            Registrar.RegisterHandler<TRequest, TResponse>(handler);
            return this;
        }
    }
}
