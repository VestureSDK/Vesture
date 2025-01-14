using NUnit.Framework;

namespace Crucible.Testing.Annotations
{
    /// <summary>
    /// Defines a test as being for an functional "conformance"
    /// </summary>
    public class ConformanceTestAttribute : CategoryAttribute
    {
        /// <inheritdoc cref="ImplementationTestAttribute"/>
        public ConformanceTestAttribute()
            : base("Conformance") { }
    }
}
