using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class NoHandlerPipelineBuilder<TInvocationRequest, TInvocationResponse> : RootMediatorBuilder
    {
        public NoHandlerPipelineBuilder(IDependencyInjectionRegistrar registrar)
            : base(registrar) { }

        public NoHandlerPipelineBuilder<TInvocationRequest, TInvocationResponse> AddMiddleware<TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TInvocationRequest, TInvocationResponse>
        {
            Registrar.RegisterMiddleware<TInvocationRequest, TInvocationResponse, TMiddleware>(order);
            return this;
        }

        public NoHandlerPipelineBuilder<TInvocationRequest, TInvocationResponse> AddMiddleware<TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TInvocationRequest, TInvocationResponse>
        {
            Registrar.RegisterMiddleware<TInvocationRequest, TInvocationResponse>(middleware, order);
            return this;
        }
    }
}
