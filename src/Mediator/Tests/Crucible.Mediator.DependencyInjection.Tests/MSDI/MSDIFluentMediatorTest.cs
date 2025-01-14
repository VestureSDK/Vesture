using Crucible.Mediator.DependencyInjection.Fluent;
using Crucible.Mediator.DependencyInjection.MSDI;
using Crucible.Mediator.DependencyInjection.Tests.Fluent;
using Crucible.Testing.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.DependencyInjection.Tests.MSDI
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
            _serviceProvider = _services.BuildServiceProvider();
            return _serviceProvider.GetRequiredService<IMediator>();
        }
    }
}
