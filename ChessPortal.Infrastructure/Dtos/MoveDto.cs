using ChessPortal.Logic.Chess;
using System;

namespace ChessPortal.Infrastructure.Dtos
{
    public class MoveDto
    {
        public Guid Id { get; set; }
        public int FromX { get; set; }
        public int ToX { get; set; }
        public int FromY { get; set; }
        public int ToY { get; set; }
        public Piece Piece { get; set; }
        public Color Color { get; set; }
        public int MoveNumber { get; set; }
        public Piece? PromoteTo { get; set; }
        public Guid ChallengeId { get; set; }
    }
}
