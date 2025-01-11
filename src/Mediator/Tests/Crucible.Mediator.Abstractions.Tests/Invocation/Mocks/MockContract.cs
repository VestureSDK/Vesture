namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockContract
    {
        private readonly Dictionary<object, object> _values = [];

        public T GetValue<T>(object key)
        {
            if (_values.TryGetValue(key, out var v) && v is T value)
            {
                return value;
            }
            else
            {
                return default!;
            }
        }

        public void SetValue(object key, object value)
        {
            _values[key] = value;
        }
    }
}
