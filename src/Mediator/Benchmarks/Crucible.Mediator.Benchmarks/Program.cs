using BenchmarkDotNet.Running;

namespace Crucible.Mediator.Benchmarks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Contains("local-test"))
            {
                var benchmark = new RequestResponsePatternBenchmark();
                benchmark.GlobalSetup();
                ;
                for (int i = 0; i < 1/*1000000*/; i++)
                {
                    await benchmark.Crucible().ConfigureAwait(false);
                }
                ;
            }
            else
            {
                var summary = BenchmarkRunner.Run<RequestResponsePatternBenchmark>();
                await Task.CompletedTask;
            }
        }
    }
}
