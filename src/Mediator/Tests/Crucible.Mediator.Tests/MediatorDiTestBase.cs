using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Tests
{
    public class MediatorDiTestBase<TSut> : IDisposable
        where TSut : class
    {
        protected IServiceCollection Services { get; }

        private Lazy<ServiceProvider> ServiceProviderInitializer { get; }

        private ServiceProvider ServiceProvider => ServiceProviderInitializer.Value;

        protected MediatorDiBuilder MediatorDiBuilder { get; }

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected Lazy<TSut> SutInitializer;

        protected TSut Sut => SutInitializer.Value;

        public MediatorDiTestBase()
        {
            Services = new ServiceCollection();
            MediatorDiBuilder = Services.AddMediator();

            ServiceProviderInitializer = new Lazy<ServiceProvider>(() => Services.BuildServiceProvider());
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            SutInitializer = new Lazy<TSut>(() => ServiceProvider.GetRequiredService<TSut>());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        public virtual void Dispose()
        {
            if (ServiceProviderInitializer.IsValueCreated)
            {
                ServiceProviderInitializer.Value.Dispose();
            }
        }
    }
}
