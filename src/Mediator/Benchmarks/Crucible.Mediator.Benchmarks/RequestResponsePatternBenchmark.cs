using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NativeAot90)]
    public class RequestResponsePatternBenchmark
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private ServiceProvider _serviceProvider;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [GlobalSetup]
        public void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<SampleVanillaHandler>();

            // MediatR
            // serviceCollection.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<RequestResponsePatternBenchmark>());

            // Crucible
            serviceCollection.AddMediator()
                .Request<SampleRequest, SampleResponse>()
                    .HandleWith<SampleHandler>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Benchmark(Baseline = true)]
        public async Task NoDi()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var handler = new SampleVanillaHandler();
            var response = await handler.ExecuteAsync(request);
        }

        [Benchmark]
        public async Task SimpleDi()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var handler = _serviceProvider.GetRequiredService<SampleVanillaHandler>();
            var response = await handler.ExecuteAsync(request);
        }

        [Benchmark]
        public async Task Crucible()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            var response = await mediator.ExecuteAsync(request);
        }

        //[Benchmark]
        //public async Task MediatR()
        //{
        //    var request = new SampleRequest() { Value = "Sample value" };
        //    var mediator = _serviceProvider.GetRequiredService<MediatR.IMediator>();
        //    var response = await mediator.Send(request);
        //}

        public class SampleRequest : IRequest<SampleResponse>, MediatR.IRequest<SampleResponse>
        {
            public string? Value { get; set; }
        }

        public class SampleResponse
        {
            public string? Value { get; set; }
        }

        public class SampleHandler : RequestHandler<SampleRequest, SampleResponse>, MediatR.IRequestHandler<SampleRequest, SampleResponse>
        {
            public Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handler called");
                return Task.FromResult(new SampleResponse { Value = request.Value });
            }

            protected override Task<SampleResponse> ExecuteAsync(SampleRequest request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handler called");
                return Task.FromResult(new SampleResponse { Value = request.Value });
            }
        }

        public class SampleVanillaHandler
        {
            public Task<SampleResponse> ExecuteAsync(SampleRequest request, CancellationToken cancellationToken = default)
            {
                Console.WriteLine("Handler called");
                return Task.FromResult(new SampleResponse { Value = request.Value });
            }
        }
    }
}
