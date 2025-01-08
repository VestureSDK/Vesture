using Crucible.Mediator;
using Crucible.Mediator.DependencyInjection;
using Crucible.Mediator.Engine.DependencyInjection;

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
        /// <returns>The <see cref="RootMediatorBuilder"/> for further configuration.</returns>
        public static RootMediatorBuilder AddMediator(this IServiceCollection services)
        {
            // Returns the builder for chaining
            return new RootMediatorBuilder(new MSDIDependencyInjectionRegistrar(services));
        }
    }
}
