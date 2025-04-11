using SampleConsole.TennisGame.Contracts;
using Vesture.Mediator.Requests;

namespace SampleConsole.TennisGame
{
    internal class TennisBallHitHandler : RequestHandler<TennisBallHit, TennisBallHitResult>
    {
        private readonly Random _random;

        public TennisBallHitHandler(Random random)
        {
            _random = random;
        }

        public override Task<TennisBallHitResult> HandleAsync(TennisBallHit hit, CancellationToken cancellationToken = default)
        {
            // Reduce the player's stamina
            // and calculate the possibility
            // for each type of result
            ConsumeSenderStamina(hit.Sender);
            var hitType = CalculateHitResult(hit.Sender);

            // Returns the computed result
            var result = new TennisBallHitResult(hit, hitType);
            return Task.FromResult(result);
        }

        private void ConsumeSenderStamina(Player sender)
        {
            sender.ConsumeStamina(1);
        }

        private TennisBallHitResult.HitType CalculateHitResult(Player sender)
        {
            var options = new[] {
                TennisBallHitResult.HitType.Winner,
                TennisBallHitResult.HitType.Winner,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.InPlay,
                TennisBallHitResult.HitType.Fault,
            };

            var index = _random.Next(0, options.Length);

            return options[index];
        }
    }
}
