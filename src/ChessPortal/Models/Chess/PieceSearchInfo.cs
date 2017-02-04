using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
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
