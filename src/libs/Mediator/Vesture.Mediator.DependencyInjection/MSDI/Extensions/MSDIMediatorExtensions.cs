using Vesture.Mediator;
using Vesture.Mediator.DependencyInjection.Fluent;
using Vesture.Mediator.DependencyInjection.MSDI;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <exclude />
    /// <summary>
    /// Extension methods related to <see cref="IMediator"/> for dependency injection.
    /// </summary>
    public static class MSDIMediatorExtensions
    {
        /// <summary>
        /// Adds the <see cref="IMediator"/> to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The <see cref="RootFluentMediatorComponentRegistrar"/> for further configuration.</returns>
        public static RootFluentMediatorComponentRegistrar AddMediator(
            this IServiceCollection services
        )
        {
            // Returns the builder for chaining
            return new RootFluentMediatorComponentRegistrar(
                new MSDIMediatorComponentRegistrar(services)
            );
        }
    }
}
