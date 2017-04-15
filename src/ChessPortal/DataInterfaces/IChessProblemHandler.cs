using ChessPortal.Handlers;
using ChessPortal.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.DataInterfaces
{
    public interface IChessProblemHandler
    {
        Task<string> GetChessProblemPositionForPlayer(string playerId);
        Task<TryMoveResult> TryMove(MoveDto move, string playerId);
        bool ChessProblemExistsForPlayer(string playerId);
    }
}
