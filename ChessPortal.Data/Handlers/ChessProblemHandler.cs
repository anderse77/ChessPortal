using ChessPortal.Data.Entities;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using ChessPortal.Logic.Chess;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.Data.Handlers
{
    public class ChessProblemHandler : IChessProblemHandler
    {
        private readonly IChessPortalRepository _chessPortalRepository;
        private readonly IChessProblemService _chessProblemService;
        private const string SaveFailed = "Error: Save failed";

        public ChessProblemHandler(
            IChessPortalRepository chessPortalRepository,
            IChessProblemService chessProblemService)
        {
            _chessPortalRepository = chessPortalRepository;
            _chessProblemService = chessProblemService;
        }

        public async Task<string> GetChessProblemPositionForPlayer(string playerId)
        {
            var problemEntry = _chessPortalRepository.GetChessProblemForPlayer(playerId);
            ChessProblemResponseDto response;
            if (problemEntry != null)
            {
                response = await _chessProblemService.GetChessProblemAsync(
                    new ChessProblemRequestDto
                    {
                        Type = "explore",
                        Id = problemEntry.ChessProblemId
                    });
            }
            else
            {
                response = await _chessProblemService.GetChessProblemAsync(
                    new ChessProblemRequestDto
                    {
                        Type = "rated"
                    });
                _chessPortalRepository.AddChessProblem(
                    new ChessProblemEntity
                    {
                        ChessProblemId = response.Data.Id,
                        PlayerId = playerId,
                        MoveOffsetNumber = 0
                    });
                if (!_chessPortalRepository.Save())
                {
                    return SaveFailed;
                }
                problemEntry = _chessPortalRepository.GetChessProblemForPlayer(playerId);
            }
            return GetUpdatedGame(problemEntry, response).History.Last().ToFenString();
        }

        public async Task<TryMoveResult> TryMove(MoveDto move, string playerId)
        {
            var chessProblemEntity = _chessPortalRepository.GetChessProblemForPlayer(playerId);
            var chessProblemResponse = await _chessProblemService.GetChessProblemAsync(
                new ChessProblemRequestDto
                {
                    Type = "explore",
                    Id = chessProblemEntity.ChessProblemId
                });
            var game = GetUpdatedGame(chessProblemEntity, chessProblemResponse);
            if (!game.MakeMove(new Move(move.Piece, move.FromX, move.ToX, move.FromY, move.ToY, move.Color, move.PromoteTo)))
            {
                return TryMoveResult.InvalidMove;
            }
            var fenAfterMove = game.History.Last().ToFenString();
            game = GetUpdatedGame(chessProblemEntity, chessProblemResponse);
            game.UpdateGame(chessProblemResponse.Data.ForcedLine[chessProblemEntity.MoveOffsetNumber]);
            var correctFen = game.History.Last().ToFenString();
            ChessPlayer player;
            if (fenAfterMove == correctFen)
            {

                if (chessProblemEntity.MoveOffsetNumber + 1 == chessProblemResponse.Data.ForcedLine.Length)
                {
                    _chessPortalRepository.DeleteChessProblem(chessProblemEntity.Id);
                    if (!_chessPortalRepository.Save())
                    {
                        return TryMoveResult.Error;
                    }
                    player = _chessPortalRepository.GetPlayerById(playerId);
                    player.NumberOfProblemsSolved += 1;
                    if (!await _chessPortalRepository.UpdateUser(player))
                    {
                        return TryMoveResult.Error;
                    }
                    return TryMoveResult.SuccessProblemSolved;
                }

                chessProblemEntity.MoveOffsetNumber = chessProblemEntity.MoveOffsetNumber + 2;
                if (!_chessPortalRepository.Save())
                {
                    return TryMoveResult.Error;
                }
                return TryMoveResult.SuccessProblemNotSolved;

            }
            _chessPortalRepository.DeleteChessProblem(chessProblemEntity.Id);
            player = _chessPortalRepository.GetPlayerById(playerId);
            player.NumberOfProblemsFailed += 1;
            if (!await _chessPortalRepository.UpdateUser(player))
            {
                return TryMoveResult.Error;
            }
            if (!_chessPortalRepository.Save())
            {
                return TryMoveResult.Error;
            }
            return TryMoveResult.Failed;
        }

        public bool ChessProblemExistsForPlayer(string playerId)
        {
            return _chessPortalRepository.ChessProblemExistsForPlayer(playerId);
        }

        ChessGame GetUpdatedGame(ChessProblemEntity chessProblemEntity, ChessProblemResponseDto chessProblemResponseDto)
        {
            var game = new ChessGame(ChessPosition.FromFen(chessProblemResponseDto.Data.FenBefore));
            game.UpdateGame(chessProblemResponseDto.Data.BlunderMove);
            for (int i = 0; i < chessProblemEntity.MoveOffsetNumber; i++)
            {
                game.UpdateGame(chessProblemResponseDto.Data.ForcedLine[i]);
            }
            game.GameStatus = GameStatus.Ongoing;
            return game;
        }
    }
}
