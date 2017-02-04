using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChessPortal.Models.Chess
{
    

    public class ChessMethods
    {
        public Square[,] ChessBoard { get; set; }

        public bool WhitesTurn { get; set; }

        public EnPassantInfo EnPassantInfo { get; set; }

        public int NumberOfMovesWithoutCapturesOrPawnMoves { get; set; }

        public List<Square[,]> Positions { get; set; }

        public ChessMethods()
        {
            ChessBoard = new[,]
            {
                {
                    new Square(Piece.Rook, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black), new Square(Piece.Rook, Color.Black)
                },
                {
                    new Square(Piece.Knight, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black), new Square(Piece.Knight, Color.Black)
                },
                {
                    new Square(Piece.Bishop, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black),new Square(Piece.Bishop, Color.Black)
                },
                {
                    new Square(Piece.Queen, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black),new Square(Piece.Queen, Color.Black)
                },
                {
                    new Square(Piece.King, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black),new Square(Piece.King, Color.Black)
                },
                {
                    new Square(Piece.Bishop, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black),new Square(Piece.Bishop, Color.Black)
                },
                {
                    new Square(Piece.Knight, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black), new Square(Piece.Knight, Color.Black)
                },
                {
                    new Square(Piece.Rook, Color.White), new Square(Piece.Pawn, Color.White),
                    new Square(), new Square(),
                    new Square(), new Square(),
                    new Square(Piece.Pawn, Color.Black), new Square(Piece.Rook, Color.Black)
                }
            };

            WhitesTurn = true;
            Positions = new List<Square[,]>();
        }

        public bool TryMove(int fromX, int fromY, int toX, int toY)
        {
            return TryMove(fromX, fromY, toX, toY, Piece.Queen);
        }
        
        public bool TryMove(int fromX, int fromY, int toX, int toY, Piece promotionPiece)
        {
            var square = ChessBoard[fromX, fromY];

            var direction = GetDirection(fromX, fromY, toX, toY);

            if (!MoveCanBeValid(fromX, fromY, toX, toY, direction, square))
            {
                return false;
            }


            int moveLength = GetMoveLength(fromX, fromY, toX, toY, direction);
            switch (square.Piece)
            {
                case Piece.Pawn:
                    return TryMovePawn(moveLength, direction, fromX, fromY, toX, toY, promotionPiece);
                case Piece.Rook:
                    return TryMoveRook(moveLength, direction, fromX, fromY, toX, toY);
                case Piece.Knight:
                    return TryMoveKnight(direction, fromX, fromY, toX, toY);
                case Piece.Bishop:
                    return TryMoveBishop(moveLength, direction, fromX, fromY, toX, toY);
            }
        }

        bool MoveIsValid(Piece piece, int fromX, int fromY, int toX, int toY)
        {
            if (!Directionisokay)

        }

        bool CanMove(Piece piece, int moveLength, Direction direction, int fromX, int fromY, int toX, int toY)
        {
            bool canMove = MoveDirectionIsValid(piece, direction);

            if (piece == Piece.Pawn)
            {
                
            }

            return MoveDirectionIsValid(piece, direction) &&
                && piece == 
                GetNearestSquare(fromX, fromY, );
        }

        bool MoveDirectionIsValid(Piece piece, Direction direction)
        {
            Direction[] validDirections;
            switch (piece)
            {
                case Piece.Bishop:
                    validDirections = new[]
                    {
                        Direction.Northeast,
                        Direction.Northwest,
                        Direction.Southeast,
                        Direction.Southwest
                    };
                    break;
                    case Piece.King:
                case Piece.Queen:
                    validDirections = new[]
                    {
                        Direction.North,
                        Direction.Northeast,
                        Direction.East,
                        Direction.Southwest,
                        Direction.South,
                        Direction.Southwest,
                        Direction.West,
                        Direction.Northwest
                    };
                    break;
                case Piece.Knight:
                    validDirections = new[]
                    {
                        Direction.Northnorthwest,
                        Direction.Northortheast,
                        Direction.Eastnortheast,
                        Direction.Eastsoutheast,
                        Direction.Southsoutheast,
                        Direction.Southsouthwest,
                        Direction.Westnorthwest,
                        Direction.Westsouthwest
                    };
                    break;
                    case Piece.Rook:
                    validDirections = new[]
                    {
                        Direction.North,
                        Direction.East,
                        Direction.South,
                        Direction.West
                    };
                    break;
                    case Piece.Pawn:
                    validDirections = new[]
                    {
                        Direction.North,
                        Direction.Northeast,
                        Direction.Northwest,
                    };
                    break;
                default:
                    validDirections = new Direction[1];
                    break;
            }
            return validDirections.Contains(direction);
        }

        bool CanMoveBishop(int moveLength, Direction direction, int fromX, int fromY, int toX, int toY)
        {
            Direction[] validDirections =
            {
                Direction.Northeast,
                Direction.Northwest,
                Direction.Southeast,
                Direction.Southwest
            };
            if (!validDirections.Contains(direction))
            {
                return false;
            }
            var info = GetNearestSquare(fromX, fromY, direction, moveLength);
            if (info.Square.Piece != Piece.Empty ||
                (info.NumberOfSquaresAway == moveLength && info.Square.Color == (WhitesTurn ? Color.White : Color.Black)))
            {
                return false;
            }
            return true;
        }

        bool CanMoveKnight(Direction direction, int fromX, int fromY, int toX, int toY)
        {
            Direction[] validDirections =
            {
                Direction.Northnorthwest,
                Direction.Northortheast,
                Direction.Eastnortheast,
                Direction.Eastsoutheast, 
                Direction.Southsoutheast, 
                Direction.Southsouthwest, 
                Direction.Westnorthwest,
                Direction.Westsouthwest
            };
            if (!validDirections.Contains(direction))
            {
                return false;
            }
            if (GetNearestSquare(fromX, fromY, direction, 1).Square.Color == (WhitesTurn ? Color.White : Color.Black))
            {
                return false;
            }
            return true;
        }

        bool TryMoveRook(int moveLength, Direction direction, int fromX, int fromY, int toX, int toY)
        {
            Direction[] validDirections =
            {
                Direction.North,
                Direction.South,
                Direction.East,
                Direction.West
            };
            if (!validDirections.Contains(direction))
            {
                return false;
            }
            var info = GetNearestSquare(fromX, fromY, direction, moveLength);
            if (info.Square.Piece != Piece.Empty ||
                (info.NumberOfSquaresAway == moveLength && info.Square.Color == (WhitesTurn ? Color.White : Color.Black)))
            {
                return false;
            }
            MakeMove(fromX, fromY, toX, toY);
            EnPassantInfo = new EnPassantInfo(false);
            Positions.Add(ChessBoard);
            WhitesTurn = !WhitesTurn;
            return true;
        }

        public PieceSearchInfo GetNearestSquare(int fromX, int fromY, Direction direction, int numberOfSquaresToSearch)
        {
            var modifiers = GetModifiers(direction);
            var x = fromX;
            var y = fromY;
            for (int i = 0; i < numberOfSquaresToSearch; i++)
            {
                x = x + modifiers[0];
                y = y + modifiers[1];
                var square = ChessBoard[x, y];
                if (square.Piece != Piece.Empty)
                {
                    return new PieceSearchInfo(square, i + 1);
                }
            }
            return new PieceSearchInfo(new Square(), numberOfSquaresToSearch);
        }



        bool TryMovePawn(int moveLength, Direction direction, int fromX, int fromY, int toX, int toY, Piece promotionPiece)
        {
            if (moveLength > 2 || (fromY != 1 && moveLength == 2))
            {
                return false;
            }
            if (direction == Direction.North)
            {
                if (moveLength == 2 &&
                    (ChessBoard[fromX, fromY + 1].Piece != Piece.Empty ||
                     ChessBoard[fromX, fromY + 2].Piece != Piece.Empty))
                {
                    return false;
                }

                if (moveLength == 1 && ChessBoard[fromX, fromY + 1].Piece != Piece.Empty)
                {
                    return false;
                }

               
                MakeMove(fromX, fromY, toX, toY);
                if (toY == 7)
                {
                    ChessBoard[toX, toY] = new Square(promotionPiece, ChessBoard[toX, toY].Color);
                }
                EnPassantInfo = moveLength == 2 ? new EnPassantInfo(true, toX, toY) : new EnPassantInfo(false);
                Positions.Add(ChessBoard);
                WhitesTurn = !WhitesTurn;
                return true;
            }

            if (direction == Direction.Northeast || direction == Direction.Northwest)
            {
                if (moveLength == 2)
                {
                    return false;
                }
                if (TryDoEnPassant(fromX, fromY, direction))
                {
                    return true;
                }
                if (!PawnCanCapture(fromX, fromY, direction))
                {
                    return false;
                }
                MakeMove(fromX, fromY, toX, toY);
                EnPassantInfo = new EnPassantInfo(false);
                return true;
            }
            return false;
        }

        bool PawnCanCapture(int fromX, int fromY, Direction direction)
        {
            return (direction == Direction.Northeast &&
                    ChessBoard[fromX + 1, fromY + 1].Color == (WhitesTurn ? Color.Black : Color.White)) ||
                   (direction == Direction.Northwest &&
                    ChessBoard[fromX - 1, fromY + 1].Color == (WhitesTurn ? Color.Black : Color.White));
        }

        bool TryDoEnPassant(int fromX, int fromY, Direction direction)
        {
            if (PawnMoveIsEnPassant(fromX, fromY, direction))
            {
                var square = ChessBoard[fromX, fromY];
                ChessBoard[fromX, fromY] = new Square(Piece.Empty, null);
                if (direction == Direction.Northeast)
                {
                    ChessBoard[fromX + 1, fromY] = new Square(Piece.Empty, null);
                    ChessBoard[fromX + 1, fromY + 1] = square;
                }
                else if (direction == Direction.Northwest)
                {
                    ChessBoard[fromX - 1, fromY] = new Square(Piece.Empty, null);
                    ChessBoard[fromX - 1, fromY + 1] = square;
                }
                return true;
            }
            return false;
        }

        bool PawnMoveIsEnPassant(int fromX, int fromY, Direction direction)
        {
            return EnPassantInfo.PawnhasJustMovedTwoSteps &&
                   fromY == 5 &&
                   EnPassantInfo.YCoordinateOfPawn == fromY &&
                   ((direction == Direction.Northeast && EnPassantInfo.XCoordinateOfPawn == fromX + 1) ||
                    (direction == Direction.Northwest && EnPassantInfo.XCoordinateOfPawn == fromX - 1));
        }

        void MakeMove(int fromX, int fromY, int toX, int toY)
        {
            var square = new Square(ChessBoard[fromX, fromY].Piece, ChessBoard[fromX, fromY].Color);
            ChessBoard[fromX, fromY] = new Square(Piece.Empty, null);
            ChessBoard[toX, toY] = square;
            if (square.Piece != Piece.Pawn)
            {
                EnPassantInfo = new EnPassantInfo(false);
            }
            Positions.Add(ChessBoard);
            WhitesTurn = !WhitesTurn;
        }

        void RevertLastMove()
        {
            ChessBoard = Positions[Positions.Count - 2];
            Positions.RemoveAt(Positions.Count - 1);
        }

        bool MoveCanBeValid(int fromX, int fromY, int toX, int toY, Direction direction, Square square)
        {
            if (fromX < 0 || fromX > 7 || fromY < 0 || fromY > 7 || toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                return false;
            }

            if (fromX == toX && fromY == toY)
            {
                return false;
            }

            if (square.Piece == Piece.Empty)
            {
                return false;
            }


            if ((WhitesTurn && square.Color == Color.Black) || (!WhitesTurn && square.Color == Color.White))
            {
                return false;
            }


            if (direction == Direction.Other)
            {
                return false;
            }
            return true;
        }
    }
}
