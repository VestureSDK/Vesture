using Vesture.Mediator.Requests;

namespace SampleConsole.TennisGame.Contracts
{
    internal class TennisBallHit : IRequest<TennisBallHitResult>
    {
        public Player Sender { get; }

        public Player Receiver { get; }

        public TennisBallHit(Player sender, Player receiver)
        {
            Sender = sender;
            Receiver = receiver;
        }

        public override string ToString()
        {
            return $"'{Sender}' hits the ball";
        }
    }
}
