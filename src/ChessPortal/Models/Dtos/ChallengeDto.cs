using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Models.Chess;

namespace ChessPortal.Models.Dtos
{
    public class ChallengeDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public Color Color { get; set; }
        [Required(ErrorMessage = "TimePerMove is required")]
        public int DaysPerMove { get; set; }
        public List<MoveDto> Moves { get; set; } = new List<MoveDto>();
        public List<DrawRequestDto> DrawRequests = new List<DrawRequestDto>();
    }
}
