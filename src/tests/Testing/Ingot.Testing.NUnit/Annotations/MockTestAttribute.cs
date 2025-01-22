namespace Ingot.Testing.Annotations
{
    /// <summary>
    /// Defines a test as being for a "mock" class
    /// </summary>
    public class MockTestAttribute : CategoryAttribute
    {
        /// <inheritdoc cref="MockTestAttribute"/>
        public MockTestAttribute()
            : base("Mock") { }
    }
}
