namespace Crucible.Mediator.Engine.Resolvers
{
    public class SingletonInvocationComponentResolver<TComponent> : InvocationComponentResolver<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        protected override TComponent Component => _componentInitializer.Value;

        public SingletonInvocationComponentResolver(Lazy<TComponent> componentInitializer)
        {
            _componentInitializer = componentInitializer;
        }
    }
}
