using ChessPortal.Logic.Chess;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChessPortal.Infrastructure.Dtos
{
    public class ChallengeDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public Color Color { get; set; }
        [Required(ErrorMessage = "TimePerMove is required")]
        public int DaysPerMove { get; set; }
        public ChessPlayerDto Player { get; set; }
    }
}
