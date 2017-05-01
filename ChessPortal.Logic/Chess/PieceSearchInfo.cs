namespace ChessPortal.Logic.Chess
{
    public class PieceSearchInfo
    {
        public Square Square { get; set; }
        public int NumberOfSquaresAway { get; }

        public PieceSearchInfo(Square square, int numberOfSquaresAway)
        {
            Square = square;
            NumberOfSquaresAway = numberOfSquaresAway;
        }
    }
}
