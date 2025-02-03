using Ingot.Mediator.Requests;

namespace SampleConsole.PingPong.Contracts
{
    internal sealed class Ping : IRequest<Pong>
    {
        public Ping(string from)
        {
            From = from;
        }

        public string From { get; }
    }
}
