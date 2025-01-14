using Crucible.Mediator.DependencyInjection.Fluent;
using Crucible.Mediator.Engine.Tests;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.DependencyInjection.Tests.Fluent
{
    public abstract class FluentMediatorTestBase : EngineMediatorTestBase<IMediator>
    {
        protected Lazy<RootFluentMediatorComponentRegistrar> FluentBuilderInitializer { get; }

        protected RootFluentMediatorComponentRegistrar FluentBuilder => FluentBuilderInitializer.Value;

        public FluentMediatorTestBase()
        {
            FluentBuilderInitializer = new Lazy<RootFluentMediatorComponentRegistrar>(() => CreateFluentBuilder());
        }

        protected abstract RootFluentMediatorComponentRegistrar CreateFluentBuilder();

        protected override void RegisterMiddleware<TRequest, TResponse>(int order, IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            FluentBuilder
                .Invocation<TRequest, TResponse>()
                    .AddMiddleware(middleware, order);
        }

        protected override void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            FluentBuilder
                .Invocation<TRequest, TResponse>()
                    .AddMiddleware(middleware);
        }

        protected override void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            FluentBuilder
                .Request<TRequest, TResponse>()
                    .HandleWith(handler);
        }
    }
}
