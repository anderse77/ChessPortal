using AutoMapper;
using System;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using ChessPortal.Logic.Chess;

namespace ChessPortal.Data.DtoProviders
{
    public class ChessPlayerDtoProvider : IChessPlayerDtoProvider
    {
        private readonly IChessPortalRepository _repository;
        private readonly IChallengeHandler _challengeHandler;

        public ChessPlayerDtoProvider(IChessPortalRepository repository, IChallengeHandler challengeHandler)
        {
            _repository = repository;
            _challengeHandler = challengeHandler;
        }

        public ChessPlayerDto GetOpponentForGame(Guid challengeId, string playerId)
        {
            var firstPlayer = _repository.GetPlayerForChallenge(challengeId, Color.White);
            var secondPlayer = _repository.GetPlayerForChallenge(challengeId, Color.Black);
            return firstPlayer.Id == playerId
                ? Mapper.Map<ChessPlayerDto>(secondPlayer)
                : Mapper.Map<ChessPlayerDto>(firstPlayer);
        }


        public ChessPlayerDto GetPlayer(string playerId)
        {
            return Mapper.Map<ChessPlayerDto>(_repository.GetPlayerById(playerId));
        }
    }
}
