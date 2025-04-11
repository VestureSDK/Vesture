using Microsoft.Extensions.DependencyInjection;
using SampleConsole.TennisGame;
using SampleConsole.TennisGame.Contracts;
using Vesture.Mediator;

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

            // Add a sample tennis game where Bob and
            // Alice are going to play against one another
            var bob = new Person("Bob");
            var alice = new Person("Alice");
            var game = new FriendlyTennisGame(bob, alice);
            services.AddSingleton(game);
            services.AddSingleton(new Random());

            // Add the mediator to the services
            services.AddMediator()

                // Register the Tennis handling
                .Request<TennisBallHit, TennisBallHitResult>()
                    .AddMiddleware<TennisRallyMiddleware>() // The middleware logs the exchanges
                    .HandleWith<TennisBallHitHandler>(); // The handler determines the outcome of a hit

            // Build the service provider and retrieve the IMediator
            using var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            // Get the tennis game instance and run the game until one player has 10 points
            var tennisGame = serviceProvider.GetRequiredService<FriendlyTennisGame>();
            
            var servingPlayer = tennisGame.Player1;
            while (Math.Max(tennisGame.Player1.Score, tennisGame.Player2.Score) < 10)
            {
                // Print the header for the current point being played
                var pointCount = 1 + tennisGame.Player1.Score + tennisGame.Player2.Score;
                System.Console.WriteLine();
                System.Console.WriteLine($"========== Point {pointCount} ==========");
                System.Console.WriteLine($"{tennisGame.Player1}: {tennisGame.Player1.Score}");
                System.Console.WriteLine($"{tennisGame.Player2}: {tennisGame.Player2.Score}");
                System.Console.WriteLine($"----------------------------------------");

                // Start the point and play until the rally is over
                var hittingPlayer = servingPlayer;
                var rallyOver = false;
                while (!rallyOver)
                {
                    // Create a hit from the player sending and send it to the mediator.
                    // The register handler will come back with the outcome of the hit.
                    var hit = new TennisBallHit(hittingPlayer, hittingPlayer.Opponent!);
                    var result = await mediator.ExecuteAsync(hit);

                    if (result.Type == TennisBallHitResult.HitType.Winner)
                    {
                        // The sender hit a winner shot, the receiver could
                        // not do anything; the sender is awarded a point.
                        result.Hit.Sender.IncrementScore();
                        System.Console.WriteLine($"'{result.Hit.Sender}' is awarded a point. Their score is now '{result.Hit.Sender.Score}'");
                        
                        rallyOver = true;
                    }
                    else if (result.Type == TennisBallHitResult.HitType.Fault)
                    {
                        // The sender hit a fault, the receiver is awarded a point.
                        result.Hit.Receiver.IncrementScore();
                        System.Console.WriteLine($"'{result.Hit.Receiver}' is awarded a point. Their score is now '{result.Hit.Receiver.Score}'");

                        rallyOver = true;
                    }
                    else
                    {
                        // The ball is in play, the receiver has to hit the ball
                        hittingPlayer = result.Hit.Receiver;
                    }
                }

                // After the rally is over, the serving player
                // changes to the other side
                servingPlayer = servingPlayer.Opponent!;
            }

            // Print the final results
            System.Console.WriteLine();
            System.Console.WriteLine($"========== Results ==========");
            System.Console.WriteLine($"{tennisGame.Player1}: {tennisGame.Player1.Score}");
            System.Console.WriteLine($"{tennisGame.Player2}: {tennisGame.Player2.Score}");
            System.Console.WriteLine($"------------------------------");

            // Announce the winner of the game
            var winner = tennisGame.Player1.Score > tennisGame.Player2.Score ? tennisGame.Player1 : tennisGame.Player2;
            System.Console.WriteLine();
            System.Console.WriteLine($"'{winner}' has won the friendly match this time, '{winner.Opponent}' will take their revenge next time"); 
        }
    }
}
