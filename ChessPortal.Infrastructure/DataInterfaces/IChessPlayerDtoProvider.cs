using System;
using ChessPortal.Infrastructure.Dtos;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IChessPlayerDtoProvider
    {
        ChessPlayerDto GetPlayer(string playerId);
    }
}
