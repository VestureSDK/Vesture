using Crucible.Mediator;
using Crucible.Mediator.DependencyInjection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <exclude />
    /// <summary>
    /// Extension methods related to <see cref="IMediator"/> for dependency injection.
    /// </summary>
    public static class MediatorDiExtensions
    {
        /// <summary>
        /// Adds the <see cref="IMediator"/> to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="setup">The <see cref="MediatorConfiguration"/> setup.</param>
        /// <returns>The <see cref="MediatorDiBuilder"/> for further configuration.</returns>
        public static MediatorDiBuilder AddMediator(this IServiceCollection services, Action<MediatorConfiguration>? setup = null)
        {
            // Returns the builder for chaining
            return new MediatorDiBuilder(services, setup);
        }
    }
}
