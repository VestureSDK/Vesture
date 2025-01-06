namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class SingletonInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        public SingletonInvocationComponentResolver(Lazy<TComponent> componentInitializer)
        {
            _componentInitializer = componentInitializer;
        }

        public TComponent ResolveComponent() => _componentInitializer.Value;
    }
}
