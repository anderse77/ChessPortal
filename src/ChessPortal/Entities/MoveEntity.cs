using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Entities
{
    public class MoveEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FromX { get; set; }
        public int ToX { get; set; }
        public int FromY { get; set; }
        public int ToY { get; set; }
        public DateTime MoveDate { get; set; }
        [ForeignKey("ChallengeId")]
        public ChallengeEntity Challange { get; set; }
        public int ChallengeId { get; set; }
    }
}
