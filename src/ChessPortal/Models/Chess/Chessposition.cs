using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.ExtensionMethods.ChessboardExtensions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.VisualBasic.CompilerServices;

namespace ChessPortal.Models.Chess
{

    public class ChessPosition : IEnumerable<Square>
    {
        private Square[,] _board;

        public string EnPassantCaptureLocationIfSetupFromFen { get; set; }

        public ChessPosition(Square[,] board, bool whiteToMove)
        {
            if (board.GetLength(0) != BoardCharacteristics.SideLength || board.GetLength(1) != BoardCharacteristics.SideLength)
            {
                throw new ArgumentException("This is not a valid chessposition");
            }
            _board = board;
            WhiteToMove = whiteToMove;
        }

        public Square this[int x, int y] => _board[x, y];

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            ChessPosition board = obj as ChessPosition;
            if (board == null)
            {
                return false;
            }
            return this.ContentEquals(board);
        }

        public int GetLength(int dimension)
        {
            return _board.GetLength(dimension);
        }

        public override int GetHashCode()
        {
            return _board.GetHashCode();
        }

        public static bool operator ==(ChessPosition lhs, ChessPosition rhs)
        {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(ChessPosition lhs, ChessPosition rhs)
        {
            return !Equals(lhs, rhs);
        }

        public bool WhiteToMove { get; set; }

        public ChessPosition UpdateBoardIfValid(Move move)
        {
            var newBoard = new Square[BoardCharacteristics.SideLength, BoardCharacteristics.SideLength];
            for (int i = 0; i < newBoard.GetLength(1); i++)
            {
                for (int j = 0; j < newBoard.GetLength(0); j++)
                {
                    if (PieceLeftCoordinates(move, j, i))
                    {
                        newBoard[j, i] = new Square();
                    }
                    else if (PieceWentToCoordinates(move, j, i))
                    {
                        newBoard[j, i] = MoveIsPawnPromotion(move)
                            ? new Square(move.PromoteTo ?? Piece.Queen, move.Color)
                            : new Square(move.Piece, move.Color);
                    }
                    else if (MoveIsEnPassantAndPawnIsTakenAtCoordinates(move, j, i))
                    {
                        newBoard[j, i] = new Square();
                    }
                    else if (MoveIsCastleAndRookWentToOrFromCoordinates(move, j, i))
                    {
                        if (MoveIsCatleAndRookWentFromCoordinates(move, j, i))
                        {
                            newBoard[j, i] = new Square();
                        }
                        if (MoveIsCastleAndRookWentToCoordinates(move, j, i))
                        {
                            newBoard[j, i] = new Square(Piece.Rook, move.Color);
                        }
                    }
                    else
                    {
                        newBoard[j, i] = _board[j, i];
                    }

                }
            }
            var newPostion = new ChessPosition(newBoard, move.Color == Color.Black);
            return newPostion.KingIsInCheck(move.Color) ? this : newPostion;
        }

        public static ChessPosition FromFen(string fen)
        {
            var parts = fen.Split(' ');
            if (parts.Length < 2)
            {
                throw new ArgumentException("This is not a valid fen-string");
            }
            var ranks = parts[0].Split('/');
            if (ranks.Length != BoardCharacteristics.SideLength)
            {
                throw new ArgumentException("This is not a valid fen-string");
            }
            var board = new Square[BoardCharacteristics.SideLength, BoardCharacteristics.SideLength];
            for (int i = 0; i < BoardCharacteristics.SideLength; i++)
            {
                var xCounter = 0;
                var rank = ranks[i];
                var yCounter = 7 - i;
                for (int j = 0; j < rank.Length; j++)
                {
                    int number = 0;
                    if (int.TryParse(rank[j].ToString(), out number))
                    {
                        for (int k = 0; k < number; k++)
                        {
                            board[xCounter, yCounter] = new Square();
                            xCounter++;
                        }
                    }
                    else
                    {
                        board[xCounter, yCounter] = Square.FromFenChar(rank[j]);
                        xCounter++;
                    }
                }
            }
            return new ChessPosition(board, parts[1] == "w")
            {
                EnPassantCaptureLocationIfSetupFromFen = parts.Length >= 4 && parts[3] != "-" ? parts[3] : string.Empty
            };
        }

        public string ToFenString()
        {
            var fen = new StringBuilder();
            for (int j = 7; j > -1; j--)
            {
                for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                {
                    var square = _board[i, j];
                    if (square.Piece.HasValue)
                    {
                        fen.Append(square.ToFenChar());
                    }
                    else
                    {
                        int number = 0;
                        while (i !=7 && !square.Piece.HasValue)
                        {
                            number++;
                            i++;
                            square = _board[i, j];
                        }
                        if (square.Piece.HasValue)
                        {
                            i--;
                        }
                        else
                        {
                            number++;
                        }
                        fen.Append(number.ToString());
                    }
                }
                fen.Append("/");
            }
            fen.Remove(fen.Length - 1, 1);
            fen.Append(WhiteToMove ? " w" : " b");
            return fen.ToString();
        }

        bool MoveIsCastleAndRookWentToCoordinates(Move move, int x, int y)
        {
            return x == (move.Direction == Direction.East ? move.ToX - 1 : move.ToX + 1) && y == move.ToY;
        }

        bool MoveIsCatleAndRookWentFromCoordinates(Move move, int x, int y)
        {
            return x == (move.Direction == Direction.East ? (move.ToX == 6 ? 7 : 0) : (move.ToX == 2 ? 0 : 7)) &&
                   y == (move.Color == Color.White ? 0 : 7);
        }

        bool MoveIsCastleAndRookWentToOrFromCoordinates(Move move, int x, int y)
        {
            return move.IsCastle &&
                   ((x == (move.Direction == Direction.East ? (move.ToX == 6 ? 7 : 0) : (move.ToX == 2 ? 0 : 7)) &&
                     y == (move.Color == Color.White ? 0 : 7)) ||
                    (x == (move.Direction == Direction.East ? move.ToX - 1 : move.ToX + 1) && y == move.ToY));
        }

        bool MoveIsEnPassantAndPawnIsTakenAtCoordinates(Move move, int x, int y)
        {
            return move.IsEnPassant && x == move.FromX + move.Direction.GetModifiers()[0] &&
                   y == move.FromY;
        }

        bool MoveIsPawnPromotion(Move move)
        {
            return move.Piece == Piece.Pawn &&
                   (move.Color == Color.White ? move.ToY == 7 : move.ToY == 0);
        }

        bool PieceLeftCoordinates(Move move, int x, int y)
        {
            return x == move.FromX && y == move.FromY;
        }

        bool PieceWentToCoordinates(Move move, int x, int y)
        {
            return x == move.ToX && y == move.ToY;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Square> GetEnumerator()
        {
            for (int i = 0; i < _board.GetLength(1); i++)
            {
                for (int j = 0; j < _board.GetLength(0); j++)
                {
                    yield return _board[j, i];
                }
            }
        }

        public int[] GetKingPosition(Color color)
        {
            for (int i = 0; i < _board.GetLength(1); i++)
            {
                for (int j = 0; j < _board.GetLength(0); j++)
                {
                    var square = _board[j, i];
                    if (square.Piece == Piece.King && square.Color == color)
                    {
                        return new[] { j, i };
                    }
                }
            }
            throw new Exception("King is not on board");
        }
    }
}

