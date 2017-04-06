using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Entities;
using ChessPortal.Models.Chess;

namespace ChessPortal.DataInterfaces
{
    public interface IChessPortalRepository
    {
        void AddChallenge(ChallengeEntity challenge);
        IEnumerable<ChallengeEntity> GetChallenges(string playerId);
        bool ChallengeExists(Guid challengeId);
        bool ChallengeIsAccepted(Guid challengeId);
        bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId);
        void AcceptChallenge(Guid challengeId, string playerId);
        IEnumerable<ChallengeEntity> GetAcceptedChallengesForPlayer(string playerId);
        bool ChallengeIsCreatedOrAcceptedByPlayer(Guid challengeId, string playerId);
        ChallengeEntity GetChallenge(Guid challengeId);
        ChessPlayer GetPlayerForChallenge(Guid challengeId, Color color);
        void AddMove(MoveEntity move);
        Task<bool> UpdateUser(ChessPlayer user);
        bool Save();
    }
}
