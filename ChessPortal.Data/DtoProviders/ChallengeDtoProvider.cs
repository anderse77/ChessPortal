using AutoMapper;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using System.Collections.Generic;

namespace ChessPortal.Infrastructure.DtoProviders
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
            return Mapper.Map<IEnumerable<ChallengeDto>>(_repository.GetChallengesThatPlayerCanAccept(playerId));
        }
    }
}
