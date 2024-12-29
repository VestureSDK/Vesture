using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Tests
{
    public class MediatorDiTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSut> : IDisposable
        where TSut : class
    {
        protected IServiceCollection Services { get; }

        private Lazy<ServiceProvider> ServiceProviderInitializer { get; }

        private ServiceProvider ServiceProvider => ServiceProviderInitializer.Value;

        protected MediatorDiBuilder MediatorDiBuilder { get; }

        protected Lazy<TSut> SutInitializer;

        protected TSut Sut => SutInitializer.Value;

        public MediatorDiTestBase()
        {
            Services = new ServiceCollection();
            MediatorDiBuilder = Services.AddMediator();

            ServiceProviderInitializer = new Lazy<ServiceProvider>(() => Services.BuildServiceProvider());
            SutInitializer = new Lazy<TSut>(() => ServiceProvider.GetRequiredService<TSut>());
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
