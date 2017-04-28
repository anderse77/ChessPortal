using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessPortal.Entities;

namespace ChessPortal.DataInterfaces
{
    public interface IGameDtoProvider
    {
        IEnumerable<GameDto> GetGames(string playerId);
    }
}
