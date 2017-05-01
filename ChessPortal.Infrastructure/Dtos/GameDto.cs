using ChessPortal.Logic.Chess;
using System;
using System.Collections.Generic;

namespace ChessPortal.Infrastructure.Dtos
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
