using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Crucible.Mediator.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net90)]
    // [SimpleJob(RuntimeMoniker.NativeAot90)]
    public class RequestResponsePatternBenchmark
    {
        private IMediator _mediator;
        private MediatR.IMediator _mediatr;
        private SampleHandler _handler;
        private SampleRequest _request;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private ServiceProvider _serviceProvider;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [GlobalSetup]
        public void GlobalSetup()
        {
            _handler = new SampleHandler();
            _request = new SampleRequest() { Value = "Sample value" };

            var serviceCollection = new ServiceCollection();

            // serviceCollection.AddSingleton<SampleHandler>();

            // MediatR
            serviceCollection.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<RequestResponsePatternBenchmark>());

            // Crucible
            serviceCollection.AddMediator()
                .Request<SampleRequest, SampleResponse>()
                    .HandleWith(_handler);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
            _mediatr = _serviceProvider.GetRequiredService<MediatR.IMediator>();

            // test = new Test(_serviceProvider);

            Crucible().GetAwaiter().GetResult();
            MediatR().GetAwaiter().GetResult();
        }

        [Benchmark]
        public Task Vanilla()
        {
            return _handler.Handle(_request);
        }

        [Benchmark]
        public Task Crucible()
        {
            return _mediator.ExecuteAsync(_request);
        }

        //[Benchmark]
        //public void OtherTest2()
        //{
        //    otherTest.A(_request);
        //}

        //[Benchmark]
        //public ValueTask SampleTest()
        //{
        //    return new ValueTask(test.ExecuteAsync(_request));
        //}

        [Benchmark(Baseline = true)]
        public Task MediatR()
        {
            return _mediatr.Send(_request);
        }

        public class SampleRequest : IRequest<SampleResponse>, MediatR.IRequest<SampleResponse>
        {
            private readonly static TimeSpan WaitTime = TimeSpan.FromMilliseconds(100);

            public string? Value { get; set; }

            internal static async Task<SampleResponse> GetResponseAsync(SampleRequest request, CancellationToken cancellationToken = default)
            {
                await Task.Delay(WaitTime).ConfigureAwait(false);
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
                return HandleAsync(request, cancellationToken);
            }

            public override Task<SampleResponse> HandleAsync(SampleRequest request, CancellationToken cancellationToken)
            {
                return SampleRequest.GetResponseAsync(request, cancellationToken);
            }
        }
    }
}
