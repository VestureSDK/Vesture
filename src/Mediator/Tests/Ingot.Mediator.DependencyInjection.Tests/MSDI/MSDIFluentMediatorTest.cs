using Ingot.Mediator.DependencyInjection.Fluent;
using Ingot.Mediator.DependencyInjection.MSDI;
using Ingot.Mediator.DependencyInjection.Tests.Fluent;
using Ingot.Testing.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Ingot.Mediator.DependencyInjection.Tests.MSDI
{
    [ImplementationTest]
    public class MSDIFluentMediatorTest : FluentMediatorTestBase, IDisposable
    {
        private readonly ServiceCollection _services = new();

        private ServiceProvider? _serviceProvider;

        public void Dispose() => _serviceProvider?.Dispose();

        protected override RootFluentMediatorComponentRegistrar CreateFluentBuilder()
        {
            var registrar = new MSDIMediatorComponentRegistrar(_services);
            return new RootFluentMediatorComponentRegistrar(registrar);
        }

        protected override IMediator CreateMediator()
        {
            // Ensures initialization
            _ = FluentBuilder;

            _serviceProvider = _services.BuildServiceProvider();
            return _serviceProvider.GetRequiredService<IMediator>();
        }
    }
}
