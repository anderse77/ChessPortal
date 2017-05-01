using ChessPortal.Logic.Chess;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPortal.Data.Entities
{
    public class MoveEntity
    {
        [Key]
        public Guid Id { get; set; }
        public int FromX { get; set; }
        public int ToX { get; set; }
        public int FromY { get; set; }
        public int ToY { get; set; }
        public Piece Piece { get; set; }
        public Color Color { get; set; }
        public DateTime MoveDate { get; set; }
        public int MoveNumber { get; set; }
        public Piece? PromoteTo { get; set; }
        [ForeignKey("ChallengeId")]
        public ChallengeEntity Challenge { get; set; }
        public Guid ChallengeId { get; set; }
    }
}
