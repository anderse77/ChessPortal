using ChessPortal.Logic.Chess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPortal.Logic.Chess
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

        public static bool PawnCanCaptureEnPassant(this Move move, ChessPosition position, IList<Move> moveList)
        {
            if (moveList.Any() && RuleInfo.PawnCaptureDirections[move.Color].Contains(move.Direction))
            {
                return LastMoveEnablesEnPassant(move, moveList[moveList.Count - 1]);
            }
            return false;
        }

        public static bool KingCanCastle(this Move move, IList<Move> moveList, ChessPosition position)
        {
            if (move.PieceLengthAndDirectionAreCorrectForCastling())
            {
                if (moveList.OwnKingHasMoved(move.Color))
                {
                    return false;
                }
                if (moveList.RookParticipatingInCastlingHasMoved(move, position))
                {
                    return false;
                }
                if (!position.PathBetweenKingAndRookIsFreeForCastling(move))
                {
                    return false;
                }
                if (position.KingIsInCheckAfterCastlingOrKingDoesPassesOverASquareControlledByOpponent(move))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        static bool PieceLengthAndDirectionAreCorrectForCastling(this Move move)
        {
            return move.Piece == Piece.King && move.Length == 2 &&
                   (move.Direction == Direction.East || move.Direction == Direction.West);
        }

        static bool OwnKingHasMoved(this IList<Move> moveList, Color ownColor)
        {
            return moveList.Any(m => m.Piece == Piece.King && m.Color == ownColor);
        }

        static bool RookParticipatingInCastlingHasMoved(this IList<Move> moveList, Move move, ChessPosition position)
        {
            var nearestRookPosition = new[] { move.Direction == Direction.East ? 7 : 0, move.Color == Color.White ? 0 : 7 };
            return position[nearestRookPosition[0], nearestRookPosition[1]].Piece != Piece.Rook ||
                   moveList.Any(
                       m =>
                           m.Piece == Piece.Rook && m.Color == move.Color && m.FromX == nearestRookPosition[0] &&
                           m.FromY == nearestRookPosition[1]);
        }

        static bool PathBetweenKingAndRookIsFreeForCastling(this ChessPosition position, Move move)
        {
            var numberOfSquaresToSearch = move.Direction == Direction.East ? 2 : 3;
            var searchInfo = position.GetNearestOccupiedSquare(move.FromX, move.FromY, move.Direction,
                numberOfSquaresToSearch);
            return searchInfo.Square.Piece == null;
        }

        static bool KingIsInCheckAfterCastlingOrKingDoesPassesOverASquareControlledByOpponent(
            this ChessPosition position, Move move)
        {
            var increment = move.Direction == Direction.East ? 1 : -1;
            for (int i = 1; i <= 2; i++)
            {
                var newPosition =
                    position.UpdateBoardIfValid(new Move(Piece.King, move.FromX, move.FromX + i * increment, move.FromY,
                        move.ToY, move.Color, null));
                if (newPosition.KingIsInCheck(move.Color))
                {
                    return true;
                }
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
                case Direction.Northnortheast:
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

        public static bool KingIsInCheck(this ChessPosition position, Color color)
        {
            var ownKingPosition = position.GetKingPosition(color);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (direction != Direction.Other)
                {
                    int numberOfSquaresToNearestEdge = GetNumberOfSquaresToNearestEdge(ownKingPosition, direction);
                    var nearestOccupiedSquareInfo = position.GetNearestOccupiedSquare(ownKingPosition[0], ownKingPosition[1],
                        direction,
                        RuleInfo.ValidDirections[Piece.Knight].Contains(direction) && numberOfSquaresToNearestEdge > 0 ? 1 : numberOfSquaresToNearestEdge);
                    if (nearestOccupiedSquareInfo.Square.Piece.HasValue)
                    {
                        if (nearestOccupiedSquareInfo.Square.Piece == Piece.Pawn && nearestOccupiedSquareInfo.NumberOfSquaresAway == 1)
                        {
                            if (color == Color.White && nearestOccupiedSquareInfo.Square.Color == Color.Black)
                            {
                                if (direction == Direction.Northeast || direction == Direction.Northwest)
                                {
                                    return true;
                                }
                            }
                            else if (color == Color.Black && nearestOccupiedSquareInfo.Square.Color == Color.White)
                            {
                                if (direction == Direction.Southeast || direction == Direction.Southwest)
                                {
                                    return true;
                                }
                            }
                        }
                        else if (nearestOccupiedSquareInfo.Square.Piece != Piece.Pawn)
                        {
                            if (nearestOccupiedSquareInfo.Square.Color != color &&
                                RuleInfo.MaximumMoveLength[nearestOccupiedSquareInfo.Square.Piece.Value] >= nearestOccupiedSquareInfo.NumberOfSquaresAway &&
                                RuleInfo.ValidDirections.Keys.Any(k => k == nearestOccupiedSquareInfo.Square.Piece.Value && RuleInfo.ValidDirections[k].Contains(direction)))
                            {
                                return true;
                            }
                        }
                    }
                }
                
            }
            return false;
        }

        public static PieceSearchInfo GetNearestOccupiedSquare(this ChessPosition position, int fromX, int fromY, Direction direction, int numberOfSquaresToSearch)
        {
            var modifiers = GetModifiers(direction);
            var x = fromX;
            var y = fromY;
            for (int i = 0; i < numberOfSquaresToSearch; i++)
            {
                x = x + modifiers[0];
                y = y + modifiers[1];
                var square = position[x, y];
                if (square.Piece != null)
                {
                    return new PieceSearchInfo(square, i + 1);
                }
            }
            return new PieceSearchInfo(new Square(), numberOfSquaresToSearch);
        }

        static int GetNumberOfSquaresToNearestEdge(int[] coordinates, Direction direction)
        {
            var numberOfSquaresToNearestEdge = 0;
            var modifiers = direction.GetModifiers();
            int[] newCoordinates = {coordinates[0] + modifiers[0], coordinates[1] + modifiers[1]};
            while (newCoordinates.All(i => i > -1 &&  i < 8))
            {
                numberOfSquaresToNearestEdge++;
                newCoordinates = new[] {newCoordinates[0] + modifiers[0], newCoordinates[1] + modifiers[1]};
            }
            return numberOfSquaresToNearestEdge;
        }       

        static Direction[] GetValidPawnCaptureDirections(Color pawnColor)
        {
            return pawnColor == Color.White
                ? new[] {Direction.Northeast, Direction.Northwest}
                : new[] {Direction.Southeast, Direction.Southwest,};
        }

        static bool LastMoveEnablesEnPassant(Move move, Move lastMove)
        {
            return lastMove.Piece == Piece.Pawn && lastMove.Length == 2 && lastMove.ToY == move.FromY &&
                   lastMove.ToX == move.FromX + GetModifiers(move.Direction)[0];
        }

        public static bool IsBlockedByOtherPiecesOfSameColor(this Move move, ChessPosition position)
        {
            int[] modifiers = move.Direction.GetModifiers();
            var x = move.FromX;
            var y = move.FromY;
            for (int i = 0; i < move.Length; i++)
            {
                x = x + modifiers[0];
                y = y + modifiers[1];
                var square = position[x, y];
                if (square.Piece.HasValue)
                {
                    return i != move.Length - 1 || (i == move.Length - 1 && move.Color == square.Color);
                }
            }
            return false;
        }
    }
}
