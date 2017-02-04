using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.ExtensionMethods.ChessboardExtensions;
using Microsoft.VisualBasic.CompilerServices;

namespace ChessPortal.Models.Chess
{

    public class ChessPosition
    {
        private Square[,] _board;

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

        public ChessPosition UpdateBoard(Move move)
        {
                var newBoard = new Square[BoardCharacteristics.SideLength,BoardCharacteristics.SideLength];
                for (int i = 0; i < newBoard.GetLength(1); i++)
                {
                    for (int j = 0; j < newBoard.GetLength(0); j++)
                    {
                        if (j == move.FromX && i == move.FromY)
                        {
                            newBoard[j,i] = new Square();
                        }
                        else if (j == move.ToX && i == move.ToY)
                        {
                            newBoard[j,i] = new Square(move.Piece, move.Color);
                        }
                        else if (move.IsEnPassant && j == move.FromX + move.Direction.GetModifiers()[0] &&
                                 i == move.FromY + move.Direction.GetModifiers()[1])
                        {
                            newBoard[j,i] = new Square();
                        }
                        else if (move.IsCastle)
                        {
                            if ()
                        }
                    }
                }
            }
        }
    }
}
