using System;

namespace ChessPortal.Infrastructure.Dtos
{
    public class DrawRequestDto
    {
        public Guid ChallengeId { get; set; }
        public string PlayerId { get; set; }
    }
}
