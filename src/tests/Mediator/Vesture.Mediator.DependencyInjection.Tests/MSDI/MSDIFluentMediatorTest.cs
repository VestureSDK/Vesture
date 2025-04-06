using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Vesture.Mediator.DependencyInjection.Fluent;
using Vesture.Mediator.DependencyInjection.MSDI;
using Vesture.Mediator.DependencyInjection.Tests.Fluent;
using Vesture.Testing;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.DependencyInjection.Tests.MSDI
{
    [ImplementationTest]
    public class MSDIFluentMediatorTest : FluentMediatorTestBase, IDisposable
    {
        private readonly ServiceCollection _services = [];

        private ServiceProvider? _serviceProvider;

        public void Dispose() => _serviceProvider?.Dispose();

        protected override RootFluentMediatorComponentRegistrar CreateFluentBuilder()
        {
            _services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, NUnitTestContextLoggingProvider>()
            );
            _services.AddLogging(c => c.SetMinimumLevel(LogLevel.Trace));

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
