using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ChessPortal.DataInterfaces;
using ChessPortal.Models.Dtos;

namespace ChessPortal.DtoProviders
{
    public class ChallengeDtoProvider : IChallengeDtoProvider
    {
        private readonly IChessPortalRepository _repository;

        public ChallengeDtoProvider(IChessPortalRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ChallengeDto> GetChallengeDtos(string playerId)
        {
            return Mapper.Map<IEnumerable<ChallengeDto>>(_repository.GetChallenges(playerId));
        }

        public IEnumerable<ChallengeDto> GetGames(string playerId)
        {
            var games = Mapper.Map<IEnumerable<ChallengeDto>>(_repository.GetAcceptedChallengesForPlayer(playerId)).ToList();
            foreach (ChallengeDto game in games)
            {
                game.Moves = game.Moves.OrderBy(m => m.MoveNumber).ToList();
            }
            return games;
        }
    }
}
