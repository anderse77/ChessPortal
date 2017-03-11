using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace ChessPortal.Models.Chess
{
    public class Move
    {
        public Piece Piece { get; }
        public int FromX { get; }
        public int ToX { get; }
        public int FromY { get; }
        public int ToY { get; }
        public Color Color { get; }

        public Direction Direction
        {
            get
            {
                var differences = new[]
                {
                ToX - FromX,
                ToY - FromY
            };

                if (differences[0] == 0)
                {
                    if (differences[1] > 0)
                    {
                        return Direction.North;
                    }

                    if (differences[1] == 0)
                    {
                        return Direction.Other;
                    }
                    return Direction.South;
                }

                if (differences[1] == 0)
                {
                    if (differences[0] > 0)
                    {
                        return Direction.East;
                    }

                    if (differences[0] == 0)
                    {
                        return Direction.Other;
                    }
                    return Direction.West;
                }

                if (Math.Abs(differences[0]) == Math.Abs(differences[1]))
                {
                    if (differences[0] > 0 && differences[1] > 0)
                    {
                        return Direction.Northeast;
                    }

                    if (differences[0] > 0 && differences[1] < 0)
                    {
                        return Direction.Southeast;
                    }

                    if (differences[0] < 0 && differences[1] > 0)
                    {
                        return Direction.Northwest;
                    }

                    if (differences[0] < 0 && differences[1] < 0)
                    {
                        return Direction.Southwest;
                    }
                }

                if (differences[0] == 1 && differences[1] == 2)
                {
                    return Direction.Northortheast;
                }

                if (differences[0] == 2 && differences[1] == 1)
                {
                    return Direction.Eastnortheast;
                }

                if (differences[0] == 2 && differences[1] == -1)
                {
                    return Direction.Eastsoutheast;
                }

                if (differences[0] == 1 && differences[1] == -2)
                {
                    return Direction.Southsoutheast;
                }

                if (differences[0] == -1 && differences[1] == -2)
                {
                    return Direction.Southsouthwest;
                }

                if (differences[0] == -2 && differences[1] == -1)
                {
                    return Direction.Westsouthwest;
                }

                if (differences[0] == -2 && differences[1] == 1)
                {
                    return Direction.Westnorthwest;
                }

                if (differences[0] == -1 && differences[1] == 2)
                {
                    return Direction.Northnorthwest;
                }

                return Direction.Other;
            }

        }

        public int Length
        {
            get
            {
                switch (Direction)
                {
                    case Direction.North:
                    case Direction.Northeast:
                    case Direction.Northwest:
                        return ToY - FromY;
                    case Direction.East:
                    case Direction.Southeast:
                        return ToX - FromX;
                    case Direction.South:
                    case Direction.Southwest:
                        return FromY - ToY;
                    case Direction.West:
                        return FromX - ToX;
                    case Direction.Northnorthwest:
                    case Direction.Eastnortheast:
                    case Direction.Northortheast:
                    case Direction.Eastsoutheast:
                    case Direction.Westnorthwest:
                    case Direction.Southsoutheast:
                    case Direction.Southsouthwest:
                    case Direction.Westsouthwest:
                        return 1;
                    default:
                        return 0;
                }
            }
        }

        public bool IsPawnMove => Piece == Piece.Pawn;

        public bool IsCastle { get; set; }

        public bool IsCapture { get; set; }

        public bool IsEnPassant { get; set; }

        public Move(Piece piece, int fromX, int toX, int fromY, int toY, Color color)
        {
            Piece = piece;
            FromX = fromX;
            ToX = toX;
            FromY = fromY;
            ToY = toY;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            Move move = obj as Move;
            if (move == null)
            {
                return false;
            }
            return FromX == move.FromX && FromY == move.FromY && ToX == move.ToX && ToY == move.ToY;
        }

        public override int GetHashCode()
        {
            return FromX.GetHashCode() ^ FromY.GetHashCode() ^ ToX.GetHashCode() ^ ToY.GetHashCode();
        }

        public static bool operator ==(Move lhs, Move rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if ((object) lhs == null || (object) rhs == null)
            {
                return false;
            }

            return lhs.FromX == rhs.FromX && lhs.FromY == rhs.FromY && lhs.ToX == rhs.ToX && lhs.ToY == rhs.ToY;
        }

        public static bool operator !=(Move lhs, Move rhs)
        {
            return !(lhs == rhs);
        }
    }
}
