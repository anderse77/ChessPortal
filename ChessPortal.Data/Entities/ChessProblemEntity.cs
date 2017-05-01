using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessPortal.Data.Entities
{
    public class ChessProblemEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ChessProblemId { get; set; }
        public int MoveOffsetNumber { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public string PlayerId { get; set; }
    }
}
