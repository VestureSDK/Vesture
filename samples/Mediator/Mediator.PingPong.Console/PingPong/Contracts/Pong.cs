namespace SampleConsole.PingPong.Contracts
{
    internal sealed class Pong
    {
        public Pong(string from)
        {
            From = from;
        }

        public string From { get; }
    }
}
