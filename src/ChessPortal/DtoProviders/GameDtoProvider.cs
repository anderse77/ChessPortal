using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChessPortal.DataInterfaces;
using ChessPortal.Entities;

namespace ChessPortal.DtoProviders
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
