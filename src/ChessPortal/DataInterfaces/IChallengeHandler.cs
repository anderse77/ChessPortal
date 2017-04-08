using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Entities;
using ChessPortal.Handlers;
using ChessPortal.Models.Chess;
using ChessPortal.Models.Dtos;

namespace ChessPortal.DataInterfaces
{
    public interface IChallengeHandler
    {
        bool SetupAndSaveChallenge(ChallengeEntity challenge, string creatorId);
        bool ChallengeExists(Guid challengeId);
        bool ChallengeIsAccepted(Guid challengeId);
        bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId);
        bool AcceptChallenge(Guid challengeId, string playerId);
        Task<bool> UpdateStats(Color? winningColor, Guid challengeId);
        bool UpdateGame(MoveDto move);
        ValidationResult ValidateMove(MoveDto moveDto, Guid challengeId, string playerId);
        ValidationResult ValidateDrawRequest(Guid challengeId, string playerId);
        ValidationResult ValidateDrawAccept(Guid challengeId, string playerId);
        bool MakeDrawRequest(DrawRequestDto drawRequestDto);
        bool DrawRequestExists(Guid challengeId);
        bool DeleteDrawRequestIfItExistsAndIsMadeByOtherPlayer(Guid challengeId, string playerId);
    }
}
