using NUnit.Framework;

namespace Ingot.Testing.Annotations
{
    /// <summary>
    /// Defines a test as being for a "sample" class
    /// </summary>
    public class SampleTestAttribute : CategoryAttribute
    {
        /// <inheritdoc cref="SampleTestAttribute"/>
        public SampleTestAttribute()
            : base("Sample") { }
    }
}
