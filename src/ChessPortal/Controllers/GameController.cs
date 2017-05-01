using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChessPortal.Web.Controllers
{
    [Route("api")]
    public class GameController : Controller
    {
        private readonly IChallengeHandler _challengeHandler;
        private readonly IChallengeDtoProvider _challengeDtoProvider;
        private readonly IGameDtoProvider _gameDtoProvider;
        private readonly IChessPlayerDtoProvider _chessPlayerDtoProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameController(IChallengeHandler challengeHandler,
            IChallengeDtoProvider challengeDtoProvider,
            IGameDtoProvider gameDtoProvider,
            IChessPlayerDtoProvider chessPlayerDtoProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _challengeHandler = challengeHandler;
            _challengeDtoProvider = challengeDtoProvider;
            _gameDtoProvider = gameDtoProvider;
            _chessPlayerDtoProvider = chessPlayerDtoProvider;
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
                var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (!_challengeHandler.SetupAndSaveChallenge(challengeDto, playerId))
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok("Challenge created");
            }

            return BadRequest(ModelState);
        }

        [HttpGet("challenge")]
        [Authorize]
        public IActionResult GetChallengesThatPlayerCanAccept()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(_challengeDtoProvider.GetChallengeDtos(playerId));
        }

        [HttpGet("challenge/{id}/opponent")]
        [Authorize]
        public IActionResult GetOpponentForChallenge(Guid id)
        {
            if (!_challengeHandler.ChallengeExists(id))
            {
                return NotFound("The challenge does not exist");
            }
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!_challengeHandler.ChallengeIsCreatedOrAcceptedByPlayer(id, playerId))
            {
                return BadRequest("This is not oe of your games");
            }
            return Ok(_chessPlayerDtoProvider.GetOpponentForGame(id, playerId));
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
            return Ok(_gameDtoProvider.GetGames(playerId));
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

            switch (await Task.Run(() => _challengeHandler.ValidateMove(move, playerId)))
            {
                case ValidationResult.BlackLostOnTime:
                    if (!await _challengeHandler.UpdateStats(true, false, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You lost on time");
                case ValidationResult.WhiteLostOnTime:
                    if (!await _challengeHandler.UpdateStats(false, false, move.ChallengeId))
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
                    if (!_challengeHandler.DeleteDrawRequestIfItExistsAndIsMadeByOtherPlayer(move.ChallengeId, playerId) || !_challengeHandler.UpdateGame(move))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
                case ValidationResult.WhiteWon:
                    if (!_challengeHandler.UpdateGame(move) || !await _challengeHandler.UpdateStats(true, false, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You won!");
                case ValidationResult.BlackWon:
                    if (!_challengeHandler.UpdateGame(move) || !await _challengeHandler.UpdateStats(false, false, move.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You won!");

            }
            return Ok("Move accepted");
        }

        [HttpPost("draw/request")]
        [Authorize]
        public IActionResult RequestDraw([FromBody] DrawRequestDto drawRequest)
        {
            if (!_challengeHandler.ChallengeExists(drawRequest.ChallengeId))
            {
                return NotFound("Game does not exist");
            }

            if (_challengeHandler.DrawRequestExists(drawRequest.ChallengeId))
            {
                return BadRequest("You cannot create a second draw request for the same challenge");
            }

            drawRequest.PlayerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (_challengeHandler.ValidateDrawRequest(drawRequest.ChallengeId, drawRequest.PlayerId))
            {
                case ValidationResult.Failed:
                    return BadRequest("You can only request a move in an ongoing game");
                    case ValidationResult.WrongPlayer:
                    return BadRequest("You must be playing the game in which you request a draw");
                    case ValidationResult.Success:
                    if (!_challengeHandler.MakeDrawRequest(drawRequest))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
            }
            return Ok("Draw request made");
        }

        [HttpPost("draw/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptDraw([FromBody] DrawAcceptDto drawAcceptDto)
        {
            if (!_challengeHandler.ChallengeExists(drawAcceptDto.ChallengeId))
            {
                return NotFound("Game does not exist");
            }

            if (!_challengeHandler.DrawRequestExists(drawAcceptDto.ChallengeId))
            {
                return NotFound("No draw request has been made for this game");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (_challengeHandler.ValidateDrawAccept(drawAcceptDto.ChallengeId, playerId))
            {
                case ValidationResult.WrongPlayer:
                    return BadRequest("You cannot accept a draw request in a game you are not playing");
                    case ValidationResult.Failed:
                    return BadRequest("You cannot accept your own draw request");
                    case ValidationResult.Success:
                    if (!await _challengeHandler.UpdateStats(false, true, drawAcceptDto.ChallengeId))
                    {
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
            }
            return Ok("Draw has been accepted");
        }
    }   
}
