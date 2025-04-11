namespace SampleConsole.TennisGame
{
    internal class Player
    {
        public decimal Stamina { get; private set; } = 100;

        public int Score { get; private set; } = 0;

        public Player? Opponent { get; set; }

        public Person Person { get; }

        public Player(Person person)
        {
            Person = person;
        }

        public void IncrementScore()
        {
            Score++;
        }

        public void ConsumeStamina(decimal consumption)
        {
            Stamina -= consumption;
        }

        public override string ToString()
        {
            return $"{Person}";
        }
    }
}
