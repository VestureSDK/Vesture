using NUnit.Framework;

namespace Crucible.Testing.Annotations
{
    /// <summary>
    /// Defines a test as being for an "implementation" class
    /// </summary>
    public class ImplementationTestAttribute : CategoryAttribute
    {
        /// <inheritdoc cref="ImplementationTestAttribute"/>
        public ImplementationTestAttribute()
            : base("Implementation") { }
    }
}
