using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPortal.Data.Entities
{
    public class DrawRequestEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public string PlayerId { get; set; }
        [ForeignKey("ChallengeId")]
        public ChallengeEntity Challenge { get; set; }
        public Guid ChallengeId { get; set; }
    }
}
