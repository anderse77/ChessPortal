using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.Entities
{
    public class ChessProblemEntity
    {
        [Key]
        public Guid id { get; set; }
        public string ChessProblemId { get; set; }
        public int moveOffsetNumber { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public string PlayerId { get; set; }
    }
}
