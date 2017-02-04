using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace ChessPortal.Models.Chess
{
    public enum Piece
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum Color
    {
        Black,
        White
    }

    public class Square
    {
        public Piece? Piece { get; }
        public Color? Color { get; }

        public Square(Piece piece, Color? color)
        {
            Piece = piece;
            Color = color;
        }

        public Square()
        {
        }

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
            Square square = obj as Square;
            if (square == null)
            {
                return false;
            }
            return Piece == square.Piece && Color == square.Color;
        }

        public static bool operator ==(Square lhs, Square rhs)
        {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(Square lhs, Square rhs)
        {
            return !Equals(lhs, rhs);
        }

        public override int GetHashCode()
        {
            return Piece.GetHashCode() ^ Color.GetHashCode();
        }
    }
}
