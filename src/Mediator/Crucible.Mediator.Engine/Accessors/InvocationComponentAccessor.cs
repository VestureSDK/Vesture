namespace Crucible.Mediator.Engine.Accessors
{
    public interface IInvocationComponentAccessor<out TComponent>
    {
        TComponent GetComponent();
    }

    public class SingletonInvocationComponentAccessor<TComponent> : IInvocationComponentAccessor<TComponent>
    {
        private readonly TComponent _component;

        public SingletonInvocationComponentAccessor(TComponent component)
        {
            _component = component;
        }

        public TComponent GetComponent() => _component;
    }

    public class LazyInvocationComponentAccessor<TComponent> : IInvocationComponentAccessor<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        public LazyInvocationComponentAccessor(Lazy<TComponent> componentInitializer)
        {
            _componentInitializer = componentInitializer;
        }

        public TComponent GetComponent() => _componentInitializer.Value;
    }

    public class InvocationComponentAccessor<TComponent> : IInvocationComponentAccessor<TComponent>
    {
        private readonly Func<TComponent> _componentFactory;

        public InvocationComponentAccessor(Func<TComponent> componentFactory)
        {
            _componentFactory = componentFactory;
        }

        public TComponent GetComponent() => _componentFactory.Invoke();
    }
}
