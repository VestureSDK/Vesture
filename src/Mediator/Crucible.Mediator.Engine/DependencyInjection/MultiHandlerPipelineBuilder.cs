using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IEvent"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MultiHandlerPipelineBuilder<TEvent> : RootMediatorBuilder
    {
        public MultiHandlerPipelineBuilder(IDependencyInjectionRegistrar registrar)
            : base(registrar)
        {
        }

        public MultiHandlerPipelineBuilder<TEvent> AddMiddleware<TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TEvent, EventResponse>
        {
            Registrar.RegisterMiddleware<TEvent, EventResponse, TMiddleware>(order);
            return this;
        }

        public MultiHandlerPipelineBuilder<TEvent> AddMiddleware<TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TEvent, EventResponse>
        {
            Registrar.RegisterMiddleware<TEvent, EventResponse>(middleware, order);
            return this;
        }

        public MultiHandlerPipelineBuilder<TEvent> HandleWith<THandler>()
            where THandler : class, IInvocationHandler<TEvent, EventResponse>
        {
            Registrar.RegisterHandler<TEvent, EventResponse, THandler>();
            return this;
        }

        public MultiHandlerPipelineBuilder<TEvent> HandleWith(IInvocationHandler<TEvent, EventResponse> handler)
        {
            Registrar.RegisterHandler<TEvent, EventResponse>(handler);
            return this;
        }

        public MultiHandlerPipelineBuilder<TEvent> PublishInParallel()
        {
            return WithPublishStrategy<ParallelHandlersStrategy<TEvent, EventResponse>>();
        }

        public MultiHandlerPipelineBuilder<TEvent> PublishSequentially()
        {
            return WithPublishStrategy<SequentialHandlersStrategy<TEvent, EventResponse>>();
        }

        public MultiHandlerPipelineBuilder<TEvent> WithPublishStrategy<TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TEvent, EventResponse>
        {
            Registrar.RegisterHandlerStrategy<TEvent, EventResponse, TStrategy>();
            return this;
        }
    }
}
