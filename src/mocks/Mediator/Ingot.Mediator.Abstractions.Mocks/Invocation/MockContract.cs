namespace Ingot.Mediator.Mocks.Invocation
{
    /// <summary>
    /// Defines a base class for mediator contracts.
    /// </summary>
    public class MockContract
    {
        private readonly Dictionary<object, object> _values = [];

        /// <summary>
        /// Gets a stored value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="key">The value key.</param>
        /// <returns>The value found or default if not.</returns>
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

        /// <summary>
        /// Store a value.
        /// </summary>
        /// <param name="key">The value key.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object key, object value)
        {
            _values[key] = value;
        }
    }
}
