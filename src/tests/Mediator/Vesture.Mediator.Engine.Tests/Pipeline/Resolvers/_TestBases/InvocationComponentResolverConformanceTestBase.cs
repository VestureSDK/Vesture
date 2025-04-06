using Vesture.Mediator.Engine.Pipeline.Resolvers;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Resolvers
{
    public abstract class InvocationComponentResolverConformanceTestBase<TComponent, TResolver>
        where TResolver : IInvocationComponentResolver<TComponent>
    {
        protected Lazy<TResolver> ResolverInitializer { get; }

        protected TResolver Resolver => ResolverInitializer.Value;

        public InvocationComponentResolverConformanceTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            ResolverInitializer = new Lazy<TResolver>(() => CreateInvocationComponentResolver());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TResolver CreateInvocationComponentResolver();
    }
}
