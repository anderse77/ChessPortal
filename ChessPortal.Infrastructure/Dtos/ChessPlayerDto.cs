namespace ChessPortal.Infrastructure.Dtos
{
    public class ChessPlayerDto
    {
        public string UserName { get; set; }
        public int NumberOfWonGames { get; set; }
        public int NumberOfLostGames { get; set; }
        public int NumberOfDrawnGames { get; set; }
        public int NumberOfProblemsSolved { get; set; }
        public int NumberOfProblemsFailed { get; set; }
    }
}
