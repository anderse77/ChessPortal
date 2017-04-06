using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ChessPortal.DataInterfaces;
using ChessPortal.Entities;
using ChessPortal.Handlers;
using ChessPortal.Models.Chess;
using ChessPortal.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessPortal.Controllers
{
    [Route("api")]
    public class GameController : Controller
    {
        private readonly IChallengeHandler _challengeHandler;
        private readonly IChallengeDtoProvider _challengeDtoProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameController(IChallengeHandler challengeHandler,
            IChallengeDtoProvider challengeDtoProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _challengeHandler = challengeHandler;
            _challengeDtoProvider = challengeDtoProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("challenge")]
        [Authorize]
        public IActionResult CreateChallenge([FromBody] ChallengeDto challengeDto)
        {
            if (challengeDto == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var challengeEntity = Mapper.Map<ChallengeEntity>(challengeDto);
                if (!_challengeHandler.SetupAndSaveChallenge(challengeEntity,
                    _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpGet("challenge")]
        [Authorize]
        public IActionResult GetChallenges()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(_challengeDtoProvider.GetChallengeDtos(playerId));
        }

        [HttpPost("challenge/{id}")]
        [Authorize]
        public IActionResult AcceptChallenge(Guid id)
        {
            if (!_challengeHandler.ChallengeExists(id))
            {
                return NotFound("The challenge you try to accept does not exist");
            }

            if (_challengeHandler.ChallengeIsAccepted(id))
            {
                return BadRequest("This challenge is already accepted");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (_challengeHandler.ChallengeIsCreatedByPlayer(id, playerId))
            {
                return BadRequest("You cannot accept your own challenges");
            }

            if (!_challengeHandler.AcceptChallenge(id, playerId))
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok();
        }

        [HttpGet("game")]
        [Authorize]
        public IActionResult GetGames()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(_challengeDtoProvider.GetGames(playerId));
        }

        [HttpPost("move")]
        [Authorize]
        public async Task<IActionResult> Move([FromBody] MoveDto move)
        {
            if (!_challengeHandler.ChallengeExists(move.ChallengeId))
            {
                return NotFound("Game does not exist");
            }

            if (!_challengeHandler.ChallengeIsAccepted(move.ChallengeId))
            {
                return BadRequest("This challenge has not yet been accepted by another player");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (await Task.Run(() => _challengeHandler.Validate(move, move.ChallengeId, playerId)))
            {
                case ValidationResult.BlackLostOnTime:
                    if (!await _challengeHandler.UpdateStats(Color.White, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You lost on time");
                case ValidationResult.WhiteLostOnTime:
                    if (!await _challengeHandler.UpdateStats(Color.Black, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You lost on time");
                case ValidationResult.OtherPlayersTurn:
                    return BadRequest("Please wait for your turn");
                case ValidationResult.WrongPlayer:
                    return BadRequest("Your are not one of the players in this game");
                case ValidationResult.WrongColor:
                    return BadRequest("Move color is not the same as your color in the game");
                case ValidationResult.InvalidPromotion:
                    return BadRequest("You can only promote a pawn that goes to the last rank");
                case ValidationResult.NotEnoughPromotionInformation:
                    return BadRequest("You must promote your pawn to another piece when it reaches the last rank");
                case ValidationResult.GameOver:
                    return BadRequest("This game is over");
                case ValidationResult.Failed:
                    return BadRequest("This is not a valid move");
                case ValidationResult.Success:
                    if (!_challengeHandler.UpdateGame(move))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
                case ValidationResult.WhiteWon:
                    if (!_challengeHandler.UpdateGame(move))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    if (!await _challengeHandler.UpdateStats(Color.White, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You won!");
                case ValidationResult.BlackWon:
                    if (!_challengeHandler.UpdateGame(move))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    if (!await _challengeHandler.UpdateStats(Color.Black, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You won!");

            }
            return Ok("Moved");
        }
    }
}
