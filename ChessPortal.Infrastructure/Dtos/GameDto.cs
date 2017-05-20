using ChessPortal.Logic.Chess;
using System;
using System.Collections.Generic;

namespace ChessPortal.Infrastructure.Dtos
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public int DaysPerMove { get; set; }
        public GameStatus Status { get; set; }
        public List<MoveDto> Moves { get; set; } = new List<MoveDto>();
        public List<DrawRequestDto> DrawRequests = new List<DrawRequestDto>();
        public ChessPlayerDto WhitePlayer { get; set; }
        public ChessPlayerDto BlackPlayer { get; set; }
        public bool WhitesTurn { get; set; }
    }
}
