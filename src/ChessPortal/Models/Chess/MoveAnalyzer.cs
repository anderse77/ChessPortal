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

        public bool MoveIsBlockedByOtherPiecesOfSameColor(Move move)
        {
            if (move.IsBlocked.HasValue && !move.IsBlocked.Value)
            {
                return false;
            }
            int[] modifiers = move.Step;
            var x = move.FromX;
            var y = move.FromY;
            for (int i = 0; i < move.Length; i++)
            {
                x = x + modifiers[0];
                y = y + modifiers[1];
                var square = Board[x, y];
                if (square.Piece.HasValue)
                {
                    move.ValidationInfo = "The move is blocked by another piece of the same color";
                    return i != move.Length - 1 || (i == move.Length - 1 && move.Color == square.Color);
                }
            }
            return false;
        }
    }
}
