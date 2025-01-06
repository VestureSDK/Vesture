namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class TransientInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        private readonly Func<TComponent> _componentFactory;

        public TransientInvocationComponentResolver(Func<TComponent> componentFactory)
        {
            _componentFactory = componentFactory;
        }

        public TComponent ResolveComponent() => _componentFactory.Invoke();
    }
}
