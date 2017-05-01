using ChessPortal.Infrastructure.Dtos;
using System.Collections.Generic;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IChallengeDtoProvider
    {
        IEnumerable<ChallengeDto> GetChallengeDtos(string playerId);
    }
}
