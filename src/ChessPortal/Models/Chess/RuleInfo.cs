using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
{
    public static class RuleInfo
    {
        public static Dictionary<Piece, Direction[]> ValidDirections = new Dictionary<Piece, Direction[]>
        {
            {Piece.Bishop, new [] {Direction.Northeast, Direction.Northwest, Direction.Southeast, Direction.Southwest}},
            {Piece.King, new [] {Direction.North, Direction.Northeast, Direction.Northwest, Direction.East, Direction.Southeast, Direction.Southwest, Direction.South, Direction.West} },
            {Piece.Knight, new [] {Direction.Eastnortheast, Direction.Northnorthwest, Direction.Southsoutheast, Direction.Southsouthwest, Direction.Westnorthwest, Direction.Westsouthwest, Direction.Northortheast, Direction.Eastsoutheast}},
            {Piece.Pawn, new [] {Direction.North, Direction.South, Direction.Northeast, Direction.Northwest, Direction.Southeast, Direction.Southwest}},
            {Piece.Queen, new [] {Direction.North, Direction.Northeast, Direction.Northwest, Direction.East, Direction.Southeast, Direction.Southwest, Direction.South, Direction.West}},
            {Piece.Rook, new [] {Direction.North, Direction.East, Direction.South, Direction.West}}
        };

        public static Dictionary<Piece, bool> CanJumpOverPieces = new Dictionary<Piece, bool>
        {
            {Piece.Bishop, false},
            {Piece.King, false},
            {Piece.Knight, true},
            {Piece.Pawn, false},
            {Piece.Queen, false},
            {Piece.Rook, false}
        };
    }
}
