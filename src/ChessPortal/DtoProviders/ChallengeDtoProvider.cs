using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ChessPortal.DataInterfaces;
using ChessPortal.Entities;
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
            return Mapper.Map<IEnumerable<ChallengeDto>>(_repository.GetChallengesThatPlayerCanAccept(playerId));
        }
    }
}
