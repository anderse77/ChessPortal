using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ChessPortal.Entities
{
    public class ChessPlayer : IdentityUser
    {
        public int NumberOfWonGames { get; set; }
        public int NumberOfLostGames { get; set; }
        public int NumberOfDrawnGames { get; set; }
        public ICollection<ChallengeEntity> Challanges { get; set; }
    }
}
