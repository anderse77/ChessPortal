using AutoMapper;
using ChessPortal.Data.Entities;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using ChessPortal.Logic.Chess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.Data.Handlers
{

    public class ChallengeHandler : IChallengeHandler
    {
        private readonly IChessPortalRepository _chessPortalRepository;

        public ChallengeHandler(IChessPortalRepository chessPortalRepository)
        {
            _chessPortalRepository = chessPortalRepository;
        }

        public bool SetupAndSaveChallenge(ChallengeDto challengeDto, string creatorId)
        {
            var challenge = Mapper.Map<ChallengeEntity>(challengeDto);
            challenge.Status = GameStatus.NotStarted;
            challenge.PlayerId = creatorId;
            _chessPortalRepository.AddChallenge(challenge);
            return _chessPortalRepository.Save();
        }

        public bool ChallengeExists(Guid challengeId)
        {
            return _chessPortalRepository.ChallengeExists(challengeId);
        }

        public bool ChallengeIsAccepted(Guid challengeId)
        {
            return _chessPortalRepository.ChallengeIsAccepted(challengeId);
        }

        public bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId)
        {
            return _chessPortalRepository.ChallengeIsCreatedByPlayer(challengeId, playerId);
        }

        public bool AcceptChallenge(Guid challengeId, string playerId)
        {
            _chessPortalRepository.AcceptChallenge(challengeId, playerId);
            return _chessPortalRepository.Save();
        }

        public ValidationResult ValidateMove(MoveDto moveDto, string playerId)
        {
            var challenge = _chessPortalRepository.GetChallenge(moveDto.ChallengeId);
            var validationResult = ValidateMoveContext(playerId, challenge, moveDto);
            if (validationResult != ValidationResult.Success)
            {
                return validationResult;
            }
            var game = new ChessGame(challenge.DaysPerMove);
            foreach (MoveEntity moveEntity in challenge.Moves)
            {
                game.MakeMove(new Move(moveEntity.Piece, moveEntity.FromX, moveEntity.ToX, moveEntity.FromY,
                    moveEntity.ToY, moveEntity.Color, moveEntity.PromoteTo));
            }
            var move = new Move(moveDto.Piece, moveDto.FromX, moveDto.ToX, moveDto.FromY, moveDto.ToY, moveDto.Color, moveDto.PromoteTo);
            if (!game.MakeMove(move))
            {
                return ValidationResult.Failed;
            }
            challenge.Status = game.GameStatus;
            if (challenge.Status == GameStatus.WhiteWins || challenge.Status == GameStatus.BlackWins)
            {
                return PlayerIsWhite(challenge, playerId) ? ValidationResult.WhiteWon : ValidationResult.BlackWon;
            }
            return ValidationResult.Success;
        }

        public async Task<bool> UpdateStats(bool whiteWins, bool draw, Guid challengeId)
        {
            var challenge = _chessPortalRepository.GetChallenge(challengeId);
            if (draw)
            {
                var player1 = _chessPortalRepository.GetPlayerForChallenge(challengeId, Color.White);
                var player2 = _chessPortalRepository.GetPlayerForChallenge(challengeId, Color.Black);
                player1.NumberOfDrawnGames += 1;
                player2.NumberOfDrawnGames += 1;
                challenge.Status = GameStatus.Draw;
                if (!await _chessPortalRepository.UpdateUser(player1) ||
                    !await _chessPortalRepository.UpdateUser(player2))
                {
                    return false;
                }
            }
            else
            {

                var winningPlayer =
                    _chessPortalRepository.GetPlayerForChallenge(challengeId, whiteWins ? Color.White : Color.Black);
                var losingPlayer = _chessPortalRepository.GetPlayerForChallenge(challengeId,
                    whiteWins ? Color.Black : Color.White);
                challenge.Status = whiteWins ? GameStatus.WhiteWins : GameStatus.BlackWins;
                winningPlayer.NumberOfWonGames += 1;
                losingPlayer.NumberOfLostGames += 1;
                if (!await _chessPortalRepository.UpdateUser(winningPlayer) ||
                    !await _chessPortalRepository.UpdateUser(losingPlayer))
                {
                    return false;
                }
            }
            
            return _chessPortalRepository.Save();
        }

        public bool UpdateGame(MoveDto move)
        {
            _chessPortalRepository.AddMove(Mapper.Map<MoveEntity>(move));
            return _chessPortalRepository.Save();
        }

        public ValidationResult ValidateDrawRequest(Guid challengeId, string playerId)
        {
            var challenge = _chessPortalRepository.GetChallenge(challengeId);
            if (challenge.Status != GameStatus.Ongoing)
            {
                return ValidationResult.Failed;
            }

            if (!ChallengeIsCreatedOrAcceptedByPlayer(challengeId, playerId))
            {
                return ValidationResult.WrongPlayer;
            }

            return ValidationResult.Success;
        }

        public ValidationResult ValidateDrawAccept(Guid challengeId, string playerId)
        {
            if (!ChallengeIsCreatedOrAcceptedByPlayer(challengeId, playerId))
            {
                return ValidationResult.WrongPlayer;
            }

            if (DrawRequestIsMadeByPlayer(challengeId, playerId))
            {
                return ValidationResult.Failed;
            }

            return ValidationResult.Success;
        }

        public bool MakeDrawRequest(DrawRequestDto drawRequestDto)
        {
            _chessPortalRepository.AddDrawRequest(Mapper.Map<DrawRequestEntity>(drawRequestDto));
            return _chessPortalRepository.Save();
        }

        public bool AcceptDrawRequest(Guid challengeId)
        {
            var challenge = _chessPortalRepository.GetChallenge(challengeId);
            challenge.Status = GameStatus.Draw;
            return _chessPortalRepository.Save();
        }

        public bool DrawRequestExists(Guid challengeId)
        {
            return _chessPortalRepository.DrawRequestExists(challengeId);
        }

        public bool DeleteDrawRequestIfItExistsAndIsMadeByOtherPlayer(Guid challengeId, string playerId)
        {
            if (DrawRequestExists(challengeId) && !DrawRequestIsMadeByPlayer(challengeId, playerId))
            {
                _chessPortalRepository.DeleteDrawRequest(challengeId);
                return _chessPortalRepository.Save();
            }
            return true;
        }

        ValidationResult ValidateMoveContext(string playerId, ChallengeEntity challenge, MoveDto moveDto)
        {
            if (!ChallengeIsCreatedOrAcceptedByPlayer(challenge.Id, playerId))
            {
                return ValidationResult.WrongPlayer;
            }
            if (challenge.Status != GameStatus.Ongoing && challenge.Status != GameStatus.NotStarted)
            {
                return ValidationResult.GameOver;
            }
            if (!PlayerHasTheMove(challenge, playerId))
            {
                return ValidationResult.OtherPlayersTurn;
            }
            if (!MoveColorIsCorrect(moveDto, challenge, playerId))
            {
                return ValidationResult.WrongColor;
            }
            if (!MoveIsPawnPromotion(moveDto) && moveDto.PromoteTo != null)
            {
                return ValidationResult.InvalidPromotion;
            }
            if (MoveIsPawnPromotion(moveDto) && (!moveDto.PromoteTo.HasValue || moveDto.PromoteTo.Value == Piece.Pawn))
            {
                return ValidationResult.NotEnoughPromotionInformation;
            }

            if (GameExpired(challenge))
            {
                var playerIsWhite = PlayerIsWhite(challenge, playerId);
                return playerIsWhite ? ValidationResult.WhiteLostOnTime : ValidationResult.BlackLostOnTime;
            }
            return ValidationResult.Success;
        }

        bool DrawRequestIsMadeByPlayer(Guid challengeId, string playerId)
        {
            return _chessPortalRepository.DrawRequestIsMadeByPlayer(challengeId, playerId);
        }

        bool ChallengeIsCreatedOrAcceptedByPlayer(Guid challengeId, string playerId)
        {
            return _chessPortalRepository.ChallengeIsCreatedOrAcceptedByPlayer(challengeId, playerId);
        }

        bool MoveIsPawnPromotion(MoveDto move)
        {
            return move.Piece == Piece.Pawn && (move.Color == Color.White ? move.ToY == 7 : move.ToY == 0);
        }

        bool MoveColorIsCorrect(MoveDto move, ChallengeEntity challenge, string playerId)
        {
            return PlayerIsWhite(challenge, playerId) ? move.Color == Color.White : move.Color == Color.Black;
        }

        bool GameExpired(ChallengeEntity challenge)
        {
            var lastMove = challenge.Moves.LastOrDefault();
            if (lastMove != null)
            {
                return lastMove.MoveDate.Add(TimeSpan.FromDays(challenge.DaysPerMove)) <= DateTime.Now;
            }
            return false;
        }

        bool PlayerIsWhite(ChallengeEntity challenge, string playerId)
        {
            return (challenge.Color == Color.White &&
                    challenge.PlayerId == playerId) ||
                   (challenge.Color == Color.Black &&
                   challenge.PlayerId != playerId);
        }

        bool PlayerHasTheMove(ChallengeEntity challenge, string playerId)
        {
            return (PlayerIsWhite(challenge, playerId) &&
                WhiteHasTheMove(challenge)) ||
                (!PlayerIsWhite(challenge, playerId) &&
                !WhiteHasTheMove(challenge));
        }

        bool WhiteHasTheMove(ChallengeEntity challenge)
        {
            return challenge.Moves.Count % 2 == 0;
        }
    }
}
