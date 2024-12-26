using BenchmarkDotNet.Running;

namespace Crucible.Mediator.Benchmarks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var benchmark = new RequestResponsePatternBenchmark();
            //benchmark.GlobalSetup();

            //for (int i = 0; i < 10000000; i++)
            //{
            //    await benchmark.Crucible();
            //}

            var summary = BenchmarkRunner.Run<RequestResponsePatternBenchmark>();
            await Task.CompletedTask;
        }
    }
}
