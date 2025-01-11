using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
    public abstract class InvocationHandlerConformanceTestBase<TRequest, TResponse, THandler>
        where THandler : IInvocationHandler<TRequest, TResponse>
    {
        protected Lazy<THandler> HandlerInitializer { get; }

        protected THandler Handler => HandlerInitializer.Value;

        protected TRequest Request { get; set; }

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected InvocationHandlerConformanceTestBase(TRequest defaultRequest)
        {
            Request = defaultRequest!;

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            HandlerInitializer = new Lazy<THandler>(() => CreateInvocationHandler());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract THandler CreateInvocationHandler();

    }
}
