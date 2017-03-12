using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChessPortal.Entities
{
    public class ChessPortalContext : IdentityDbContext<ChessPlayer>
    {
        public DbSet<ChallengeEntity> Challenges { get; set; }
        public DbSet<ChallengeAcceptEntity> AcceptedChallenges { get; set; }
        public DbSet<MoveEntity> Moves { get; set; }
    }
}
