using ChessPortal.Data.Entities;
using ChessPortal.Logic.Chess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessPortal.Data.Repositories.Interfaces
{
    public interface IChessPortalRepository
    {
        void AddChallenge(ChallengeEntity challenge);
        IEnumerable<ChallengeEntity> GetChallengesThatPlayerCanAccept(string playerId);
        bool ChallengeExists(Guid challengeId);
        bool ChallengeIsAccepted(Guid challengeId);
        bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId);
        void AcceptChallenge(Guid challengeId, string playerId);
        IEnumerable<ChallengeEntity> GetAcceptedChallengesForPlayer(string playerId);
        bool ChallengeIsCreatedOrAcceptedByPlayer(Guid challengeId, string playerId);
        ChallengeEntity GetChallenge(Guid challengeId);
        ChessPlayer GetPlayerForChallenge(Guid challengeId, Color color);
        void AddMove(MoveEntity move);
        void AddDrawRequest(DrawRequestEntity drawRequest);
        bool DrawRequestIsMadeByPlayer(Guid challengeId, string playerId);
        bool DrawRequestExists(Guid challengeId);
        void DeleteDrawRequest(Guid challengeId);
        ChessProblemEntity GetChessProblemForPlayer(string playerId);
        void AddChessProblem(ChessProblemEntity chessProblem);
        void DeleteChessProblem(Guid id);
        ChessPlayer GetPlayerById(string playerId);
        bool ChessProblemExistsForPlayer(string playerId);
        Task<bool> UpdateUser(ChessPlayer user);
        bool Save();
    }
}
