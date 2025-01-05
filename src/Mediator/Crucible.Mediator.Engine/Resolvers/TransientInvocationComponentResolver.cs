namespace Crucible.Mediator.Engine.Resolvers
{
    public class TransientInvocationComponentResolver<TComponent> : InvocationComponentResolver<TComponent>
    {
        private readonly Func<TComponent> _componentFactory;

        protected override TComponent Component => _componentFactory.Invoke();

        public TransientInvocationComponentResolver(Func<TComponent> componentFactory)
        {
            _componentFactory = componentFactory;
        }
    }
}
