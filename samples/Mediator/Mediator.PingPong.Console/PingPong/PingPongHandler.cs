using Ingot.Mediator.Requests;
using SampleConsole.PingPong.Contracts;

namespace SampleConsole.PingPong
{
    internal sealed class PingPongHandler : RequestHandler<Ping, Pong>
    {
        public override Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken = default)
        {
            var response = new Pong("Bob");
            return Task.FromResult(response);
        }
    }
}
