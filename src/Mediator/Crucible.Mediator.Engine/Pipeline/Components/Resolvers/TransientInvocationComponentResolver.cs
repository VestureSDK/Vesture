namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class TransientInvocationComponentResolver<TComponent> : InvocationComponentResolver<TComponent>
    {
        private readonly Func<TComponent> _componentFactory;

        public TransientInvocationComponentResolver(Func<TComponent> componentFactory)
        {
            _componentFactory = componentFactory;
        }

        protected override TComponent GetComponent() => _componentFactory.Invoke();
    }
}
