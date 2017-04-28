using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChessPortal.Models.Chess;
using ChessPortal.Models.Dtos;

namespace ChessPortal.Entities
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public Color Color { get; set; }
        public int DaysPerMove { get; set; }
        public List<MoveDto> Moves { get; set; } = new List<MoveDto>();
        public List<DrawRequestDto> DrawRequests = new List<DrawRequestDto>();
    }
}
