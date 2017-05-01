using ChessPortal.Infrastructure.Dtos;
using ChessPortal.Logic.Chess;
using System;
using System.Threading.Tasks;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public enum ValidationResult
    {
        Success,
        GameOver,
        WhiteWon,
        BlackWon,
        WrongPlayer,
        WrongColor,
        InvalidPromotion,
        NotEnoughPromotionInformation,
        OtherPlayersTurn,
        WhiteLostOnTime,
        BlackLostOnTime,
        Failed
    }

    public interface IChallengeHandler
    {
        bool SetupAndSaveChallenge(ChallengeDto challengeDto, string creatorId);
        bool ChallengeExists(Guid challengeId);
        bool ChallengeIsAccepted(Guid challengeId);
        bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId);
        bool AcceptChallenge(Guid challengeId, string playerId);
        Task<bool> UpdateStats(bool whiteWins, bool draw, Guid challengeId);
        bool UpdateGame(MoveDto move);
        ValidationResult ValidateMove(MoveDto moveDto, string playerId);
        ValidationResult ValidateDrawRequest(Guid challengeId, string playerId);
        ValidationResult ValidateDrawAccept(Guid challengeId, string playerId);
        bool MakeDrawRequest(DrawRequestDto drawRequestDto);
        bool DrawRequestExists(Guid challengeId);
        bool DeleteDrawRequestIfItExistsAndIsMadeByOtherPlayer(Guid challengeId, string playerId);
    }
}
