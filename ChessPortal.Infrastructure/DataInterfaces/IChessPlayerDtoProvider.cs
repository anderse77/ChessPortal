using System;
using ChessPortal.Infrastructure.Dtos;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IChessPlayerDtoProvider
    {
        ChessPlayerDto GetOpponentForGame(Guid challengeId, string playerId);
        ChessPlayerDto GetPlayer(string playerId);
    }
}
