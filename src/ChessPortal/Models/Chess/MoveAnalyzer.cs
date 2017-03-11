using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
{
    public enum Direction
    {
        North,
        South,
        East,
        West,
        Northwest,
        Northeast,
        Southwest,
        Southeast,
        Northnorthwest,
        Westnorthwest,
        Westsouthwest,
        Southsouthwest,
        Northortheast,
        Eastnortheast,
        Eastsoutheast,
        Southsoutheast,
        Other
    }

    public class MoveAnalyzer
    {
        public ChessGame ChessGame { get; }

        public MoveAnalyzer(ChessGame chessGame)
        {
            ChessGame = chessGame;
        }

        public Move AnalyzeMove(Move move)
        {
            if (move.IsValid.HasValue && !move.IsValid.Value)
            {
                return move;
            }
            if (MoveIs)
        }


    }
}
