using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Models.Chess;

namespace ChessPortal.Entities
{
    public class ChallengeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Color Color { get; set; }
        public TimeSpan TimePerMove { get; set; }
        public GameStatus Status { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public int PlayerId { get; set; }

    }
}
