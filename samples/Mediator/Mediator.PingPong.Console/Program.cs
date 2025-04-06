using Vesture.Mediator;
using Microsoft.Extensions.DependencyInjection;
using SampleConsole.PingPong;
using SampleConsole.PingPong.Contracts;

namespace SampleConsole
{
    internal sealed class Program
    {
        static async Task Main()
        {
            // Create the service collection, it can
            // also come from a Host. For more info, see
            // https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
            // https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
            var services = new ServiceCollection();

            // Add the mediator to the services
            services.AddMediator()

                // Register the Ping/Pong handling
                .Request<Ping, Pong>()
                    .HandleWith<PingPongHandler>();

            // Build the service provider and retrieve the IMediator
            using var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            // Alice sends a ping and receives a pong
            var ping = new Ping("Alice");
            Console.WriteLine($"'{ping.From}' sends a Ping");
            var pong = await mediator.ExecuteAsync(ping);
            Console.WriteLine($"'{ping.From}' received a Pong from '{pong.From}'");
        }
    }
}
