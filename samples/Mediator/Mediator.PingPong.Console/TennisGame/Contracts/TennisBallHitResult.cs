namespace SampleConsole.TennisGame.Contracts
{
    internal class TennisBallHitResult
    {
        public enum HitType
        {
            InPlay,
            Winner,
            Fault
        }

        public TennisBallHit Hit { get; }

        public HitType Type { get; }

        public TennisBallHitResult(TennisBallHit hit, HitType type)
        {
            Hit = hit;
            Type = type;
        }

        public override string ToString()
        {
            return $"The hit from '{Hit.Sender}' is '{Type}'";
        }
    }
}
