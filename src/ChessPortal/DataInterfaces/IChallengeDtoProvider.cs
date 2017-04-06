using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Models.Dtos;

namespace ChessPortal.DataInterfaces
{
    public interface IChallengeDtoProvider
    {
        IEnumerable<ChallengeDto> GetChallengeDtos(string playerId);
        IEnumerable<ChallengeDto> GetGames(string playerId);
    }
}
