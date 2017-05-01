using ChessPortal.Infrastructure.Dtos;
using System.Collections.Generic;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IGameDtoProvider
    {
        IEnumerable<GameDto> GetGames(string playerId);
    }
}
