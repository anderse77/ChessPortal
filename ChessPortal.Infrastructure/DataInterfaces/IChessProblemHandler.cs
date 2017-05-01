using ChessPortal.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public enum TryMoveResult
    {
        SuccessProblemSolved,
        SuccessProblemNotSolved,
        Failed,
        Error
    }

    public interface IChessProblemHandler
    {
        Task<string> GetChessProblemPositionForPlayer(string playerId);
        Task<TryMoveResult> TryMove(MoveDto move, string playerId);
        bool ChessProblemExistsForPlayer(string playerId);
    }
}
