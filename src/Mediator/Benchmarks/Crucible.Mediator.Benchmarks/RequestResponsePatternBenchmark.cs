using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net90)]
    public class RequestResponsePatternBenchmark
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private ServiceProvider _serviceProvider;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [GlobalSetup]
        public void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<SampleHandler>();

            // MediatR
            serviceCollection.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<RequestResponsePatternBenchmark>());

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
            var handler = new SampleHandler();
            var response = await handler.Handle(request);
        }

        [Benchmark]
        public async Task SimpleDi()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var handler = _serviceProvider.GetRequiredService<SampleHandler>();
            var response = await handler.Handle(request);
        }

        [Benchmark]
        public async Task Crucible()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            var response = await mediator.ExecuteAsync(request);
        }

        [Benchmark]
        public async Task MediatR()
        {
            var request = new SampleRequest() { Value = "Sample value" };
            var mediator = _serviceProvider.GetRequiredService<MediatR.IMediator>();
            var response = await mediator.Send(request);
        }

        public class SampleRequest : IRequest<SampleResponse>, MediatR.IRequest<SampleResponse>
        {
            public string? Value { get; set; }

            internal static async Task<SampleResponse> GetResponseAsync(SampleRequest request, CancellationToken cancellationToken)
            {
                await Task.Delay(1);
                return new SampleResponse { Value = request.Value };
            }
        }

        public class SampleResponse
        {
            public string? Value { get; set; }
        }

        public class SampleHandler : RequestHandler<SampleRequest, SampleResponse>, MediatR.IRequestHandler<SampleRequest, SampleResponse>
        {
            public Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken = default)
            {
                return ExecuteAsync(request, cancellationToken);
            }

            public override Task<SampleResponse> ExecuteAsync(SampleRequest request, CancellationToken cancellationToken)
            {
                return SampleRequest.GetResponseAsync(request, cancellationToken);
            }
        }
    }
}
