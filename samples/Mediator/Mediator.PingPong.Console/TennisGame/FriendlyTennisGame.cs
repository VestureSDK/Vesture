namespace SampleConsole.TennisGame
{
    internal class FriendlyTennisGame
    {
        public Player Player1 { get; }

        public Player Player2 { get; }

        public FriendlyTennisGame(Person player1, Person player2)
        {
            Player1 = new Player(player1);
            Player2 = new Player(player2);

            Player1.Opponent = Player2;
            Player2.Opponent = Player1;
        }
    }
}
