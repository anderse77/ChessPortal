using ChessPortal.Logic.Chess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPortal.Data.Entities
{
    public class ChallengeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Color Color { get; set; }
        public int DaysPerMove { get; set; }
        public GameStatus Status { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public string PlayerId { get; set; }
        public List<MoveEntity> Moves { get; set; } = new List<MoveEntity>();
        public List<DrawRequestEntity> DrawRequests { get; set; } = new List<DrawRequestEntity>();
    }
}
