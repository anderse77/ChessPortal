using AutoMapper;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace ChessPortal.Infrastructure.DtoProviders
{
    public class GameDtoProvider : IGameDtoProvider
    {
        private readonly IChessPortalRepository _repository;

        public GameDtoProvider(IChessPortalRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<GameDto> GetGames(string playerId)
        {
            var games = Mapper.Map<IEnumerable<GameDto>>(_repository.GetAcceptedChallengesForPlayer(playerId)).ToList();
            foreach (GameDto game in games)
            {
                game.Moves = game.Moves.OrderBy(m => m.MoveNumber).ToList();
            }
            return games;
        }
    }
}
