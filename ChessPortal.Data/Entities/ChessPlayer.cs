using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChessPortal.Data.Entities
{
    public class ChessPlayer : IdentityUser
    {
        public int NumberOfWonGames { get; set; }
        public int NumberOfLostGames { get; set; }
        public int NumberOfDrawnGames { get; set; }
        public int NumberOfProblemsSolved { get; set; }
        public int NumberOfProblemsFailed { get; set; }
        public ICollection<ChallengeEntity> Challenges { get; set; }
    }
}
