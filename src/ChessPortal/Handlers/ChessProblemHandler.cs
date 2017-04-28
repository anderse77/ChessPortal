using ChessPortal.DataInterfaces;
using ChessPortal.Entities;
using ChessPortal.Models.Chess;
using ChessPortal.Models.Chess.ChessProblems;
using ChessPortal.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChessPortal.Handlers
{
    public enum TryMoveResult
    {
        SuccessProblemSolved,
        SuccessProblemNotSolved,
        Failed,
        Error
    }

    public class ChessProblemHandler : IChessProblemHandler
    {
        private readonly IChessPortalRepository _chessPortalRepository;
        private readonly IChessProblemService _chessProblemService;
        private const string SaveFailed = "Save failed";

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
            ChessProblemResponse response;
            if (problemEntry != null)
            {
                response = await _chessProblemService.GetChessProblemAsync(
                    new ChessProblemRequest
                    {
                        Type = "explore",
                        Id = problemEntry.ChessProblemId
                    });
            }
            else
            {
                response = await _chessProblemService.GetChessProblemAsync(
                    new ChessProblemRequest
                    {
                        Type = "rated"
                    });
                _chessPortalRepository.AddChessProblem(
                    new ChessProblemEntity
                    {
                        ChessProblemId = response.Data.Id,
                        PlayerId = playerId,
                        moveOffsetNumber = 0
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
                new ChessProblemRequest
                {
                    Type = "explore",
                    Id = chessProblemEntity.ChessProblemId
                });
            var game = GetUpdatedGame(chessProblemEntity, chessProblemResponse);
            game.MakeMove(new Move(move.Piece, move.FromX, move.ToX, move.FromY, move.ToY, move.Color, move.PromoteTo));
            var fenAfterMove = game.History.Last().ToFenString();
            game = GetUpdatedGame(chessProblemEntity, chessProblemResponse);
            game.UpdateGame(chessProblemResponse.Data.ForcedLine[chessProblemEntity.moveOffsetNumber]);
            var correctFen = game.History.Last().ToFenString();
            if (fenAfterMove == correctFen)
            {

                if (chessProblemEntity.moveOffsetNumber + 1 == chessProblemResponse.Data.ForcedLine.Length)
                {
                    _chessPortalRepository.DeleteChessProblem(chessProblemEntity.Id);
                    if (!_chessPortalRepository.Save())
                    {
                        return TryMoveResult.Error;
                    }
                    var user = _chessPortalRepository.GetPlayerById(playerId);
                    user.NumberOfProblemsSolved += 1;
                    if (!await _chessPortalRepository.UpdateUser(user))
                    {
                        return TryMoveResult.Error;
                    }
                    return TryMoveResult.SuccessProblemSolved;
                }

                chessProblemEntity.moveOffsetNumber = chessProblemEntity.moveOffsetNumber + 2;
                if (!_chessPortalRepository.Save())
                {
                    return TryMoveResult.Error;
                }
                return TryMoveResult.SuccessProblemNotSolved;

            }
            _chessPortalRepository.DeleteChessProblem(chessProblemEntity.Id);
            var player = _chessPortalRepository.GetPlayerById(playerId);
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

        ChessGame GetUpdatedGame(ChessProblemEntity chessProblemEntity, ChessProblemResponse chessProblemResponse)
        {
            var game = new ChessGame(ChessPosition.FromFen(chessProblemResponse.Data.FenBefore));
            game.UpdateGame(chessProblemResponse.Data.BlunderMove);
            for (int i = 0; i < chessProblemEntity.moveOffsetNumber; i++)
            {
                game.UpdateGame(chessProblemResponse.Data.ForcedLine[i]);
            }
            game.GameStatus = GameStatus.Ongoing;
            return game;
        }
    }
}
