using Crucible.Mediator.Engine.Pipeline.Resolvers;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Bases
{
    public abstract class InvocationComponentResolverTestBase<TComponent, TResolver>
        where TResolver : IInvocationComponentResolver<TComponent>
    {
        protected Lazy<TResolver> ResolverInitializer { get; }

        protected TResolver Resolver => ResolverInitializer.Value;

        public InvocationComponentResolverTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            ResolverInitializer = new Lazy<TResolver>(() => CreateResolver());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TResolver CreateResolver();
    }
}
