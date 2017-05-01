using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChessPortal.Data.Entities
{
    public class ChessPortalContext : IdentityDbContext<ChessPlayer>
    {
        public ChessPortalContext(DbContextOptions<ChessPortalContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<ChallengeEntity> Challenges { get; set; }
        public DbSet<ChallengeAcceptEntity> AcceptedChallenges { get; set; }
        public DbSet<MoveEntity> Moves { get; set; }
        public DbSet<DrawRequestEntity> DrawRequests { get; set; }
        public DbSet<ChessProblemEntity> ChessProblems { get; set; }
    }
}
