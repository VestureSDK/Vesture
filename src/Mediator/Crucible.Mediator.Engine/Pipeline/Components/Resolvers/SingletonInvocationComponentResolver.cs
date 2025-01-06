namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class SingletonInvocationComponentResolver<TComponent> : InvocationComponentResolver<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        public SingletonInvocationComponentResolver(Lazy<TComponent> componentInitializer)
        {
            _componentInitializer = componentInitializer;
        }

        protected override TComponent GetComponent() => _componentInitializer.Value;
    }
}
