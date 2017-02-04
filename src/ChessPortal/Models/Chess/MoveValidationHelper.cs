using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
{
    public static class MoveValidationHelper
    {
        public static bool CoordinatesAreValid(this Move move)
        {
            return move.FromX > 0 && move.FromX < 7 && move.FromY > 0 && move.FromY < 7;
        }

        public static bool HasValidDirection(this Move move)
        {
            return RuleInfo.ValidDirections[move.Piece].Contains(move.Direction);
        }

        public static bool PawnCanCapture(this Move move, ChessPosition position)
        {
            if (move.Piece == Piece.Pawn &&
                GetValidPawnCaptureDirections(move.Color).Contains(move.Direction))
            {
                int[] modifiers = move.Direction.GetModifiers();
                return position[move.FromX + modifiers[0], move.FromY + modifiers[1]].Color != move.Color;
            }
            return false;
        }

        public static bool PawnCanCaptureEnPassant(this Move move, ChessPosition position, List<Move> moveList)
        {
            if (GetValidPawnCaptureDirections(move.Color).Contains(move.Direction))
            {
                return LastMoveEnablesEnPassant(move, moveList[moveList.Count - 1]);
            }
            return false;
        }

        public static int[] GetModifiers(this Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return new[] { 1, 0 };
                case Direction.Eastnortheast:
                    return new[] { 2, 1 };
                case Direction.Eastsoutheast:
                    return new[] { 2, -1 };
                case Direction.North:
                    return new[] { 0, 1 };
                case Direction.Southsouthwest:
                    return new[] { -1, -2 };
                case Direction.Northeast:
                    return new[] { 1, 1 };
                case Direction.Northnorthwest:
                    return new[] { -1, 2 };
                case Direction.Northortheast:
                    return new[] { 1, 2 };
                case Direction.South:
                    return new[] { 0, -1 };
                case Direction.West:
                    return new[] { -1, 0 };
                case Direction.Northwest:
                    return new[] { -1, 1 };
                case Direction.Westsouthwest:
                    return new[] { -2, -1 };
                case Direction.Southeast:
                    return new[] { 1, -1 };
                case Direction.Southwest:
                    return new[] { -1, -1 };
                case Direction.Southsoutheast:
                    return new[] { 1, -2 };
                case Direction.Westnorthwest:
                    return new[] { -2, 1 };
                default:
                    return new[] { 0, 0 };
            }
        }

        static Direction[] GetValidPawnCaptureDirections(Color pawnColor)
        {
            return pawnColor == Color.White
                ? new[] {Direction.Northeast, Direction.Northwest}
                : new[] {Direction.Southeast, Direction.Southwest,};
        }

        static Direction GetValidPawnMoveDirection(Color pawnColor)
        {
            return pawnColor == Color.White ? Direction.North : Direction.South;
        }

        static bool LastMoveEnablesEnPassant(Move move, Move lastMove)
        {
            return lastMove.Piece == Piece.Pawn && lastMove.Length == 2 && lastMove.ToY == move.FromY &&
                   lastMove.ToX == move.FromX + GetModifiers(move.Direction)[0];
        }
    }
}
