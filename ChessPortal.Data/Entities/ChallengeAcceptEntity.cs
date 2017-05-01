using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPortal.Data.Entities
{
    public class ChallengeAcceptEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer AcceptingPlayer { get; set; }
        [ForeignKey("ChallengeId")]
        public ChallengeEntity Challenge { get; set; }
        public string PlayerId { get; set; }
        public Guid ChallengeId { get; set; }
    }
}
