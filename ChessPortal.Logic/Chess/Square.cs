using System;
using System.Linq;

namespace ChessPortal.Logic.Chess
{
    [Flags]
    public enum Piece
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    [Flags]
    public enum Color
    {
        White,
        Black
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

        public static Square FromFenChar (char fen)
        {
            var color = fen.ToString().ToUpper() == fen.ToString() ? Chess.Color.White : Chess.Color.Black;
            if (fen.ToString().ToLower() == "n")
            {
                return new Square(Chess.Piece.Knight, color);
            }
            foreach (Piece piece in Enum.GetValues(typeof(Piece)))
            {                
                if (piece != Chess.Piece.Knight && piece.ToString().ToLower().First() == fen.ToString().ToLower().First())
                {
                    
                    return new Square(piece, color);
                }
            }
            throw new ArgumentException("This is not a valid fen char");
        }

        public char ToFenChar()
        {
            if (!Piece.HasValue || !Color.HasValue)
            {
                throw new ArgumentException("This square cannot be converted to a fen char");
            }
            char algebraicNotationCharacter;
            if (Piece.Value == Chess.Piece.Knight)
            {
                algebraicNotationCharacter = 'N';
            }
            else
            {
                algebraicNotationCharacter = Piece.ToString().First();
            }            
            return Color.Value == Chess.Color.White ? char.ToUpper(algebraicNotationCharacter) : char.ToLower(algebraicNotationCharacter);
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
