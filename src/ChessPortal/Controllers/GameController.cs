using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<GameController> _logger;

        public GameController(IChallengeHandler challengeHandler,
            IChallengeDtoProvider challengeDtoProvider,
            IGameDtoProvider gameDtoProvider,
            IChessPlayerDtoProvider chessPlayerDtoProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GameController> logger)
        {
            _challengeHandler = challengeHandler;
            _challengeDtoProvider = challengeDtoProvider;
            _gameDtoProvider = gameDtoProvider;
            _chessPlayerDtoProvider = chessPlayerDtoProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("challenge")]
        [Authorize]
        public IActionResult CreateChallenge([FromBody] ChallengeDto challengeDto)
        {
            if (challengeDto == null)
            {
                _logger.LogWarning("User has not sent input in correct format.");
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (!_challengeHandler.SetupAndSaveChallenge(challengeDto, playerId))
                {
                    _logger.LogError("Error occured while saving to the database.");
                    return StatusCode(500, "A problem happened while handling your request.");
                }
                _logger.LogInformation("Challenge was created.");
                return Ok("Challenge created");
            }

            return BadRequest(ModelState);
        }

        [HttpGet("challenge")]
        [Authorize]
        public IActionResult GetChallengesThatPlayerCanAccept()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var challenges = _challengeDtoProvider.GetChallengeDtos(playerId);
            _logger.LogInformation("Fetched challenges that a user can accept.");
            return Ok(challenges);
        }

        [HttpGet("challenge/{id}/opponent")]
        [Authorize]
        public IActionResult GetOpponentForChallenge(Guid id)
        {
            if (!_challengeHandler.ChallengeExists(id))
            {
                _logger.LogWarning("User tried to get opponent for challenge that does not exist.");
                return NotFound("The challenge does not exist");
            }
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!_challengeHandler.ChallengeIsCreatedOrAcceptedByPlayer(id, playerId))
            {
                _logger.LogWarning("User tried to get opponent in game that wasn't played by him or her.");
                return BadRequest("This is not one of your games");
            }
            var opponent = _chessPlayerDtoProvider.GetOpponentForGame(id, playerId);
            _logger.LogInformation("Fetched opponent for game.");
            return Ok(opponent);
        }

        [HttpPost("challenge/{id}")]
        [Authorize]
        public IActionResult AcceptChallenge(Guid id)
        {
            if (!_challengeHandler.ChallengeExists(id))
            {
                _logger.LogWarning("User tried to accept a challenge that does not exist");
                return NotFound("The challenge you try to accept does not exist");
            }

            if (_challengeHandler.ChallengeIsAccepted(id))
            {
                _logger.LogWarning("User tried to accept a challenge that is already accepted.");
                return BadRequest("This challenge is already accepted");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (_challengeHandler.ChallengeIsCreatedByPlayer(id, playerId))
            {
                _logger.LogWarning("User tried to accept a challenge he or she created.");
                return BadRequest("You cannot accept your own challenges");
            }

            if (!_challengeHandler.AcceptChallenge(id, playerId))
            {
                _logger.LogError("Error occured while saving to the database.");
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok();
        }

        [HttpGet("game")]
        [Authorize]
        public IActionResult GetGames()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var games = _gameDtoProvider.GetGames(playerId);
            _logger.LogInformation("fetched games for user.");
            return Ok(_gameDtoProvider.GetGames(playerId));
        }

        [HttpPost("move")]
        [Authorize]
        public async Task<IActionResult> Move([FromBody] MoveDto move)
        {
            if (!_challengeHandler.ChallengeExists(move.ChallengeId))
            {
                _logger.LogWarning("User tried to make a move in a game that does not exist.");
                return NotFound("Game does not exist");
            }

            if (!_challengeHandler.ChallengeIsAccepted(move.ChallengeId))
            {
                _logger.LogWarning("User tried to make a move before a challenge was accepted by another player.");
                return BadRequest("This challenge has not yet been accepted by another player");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (await Task.Run(() => _challengeHandler.ValidateMove(move, playerId)))
            {
                case ValidationResult.BlackLostOnTime:
                    if (!await _challengeHandler.UpdateStats(true, false, move.ChallengeId))
                    {
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    _logger.LogInformation("User lost on time.");
                    return Ok("You lost on time");
                case ValidationResult.WhiteLostOnTime:
                    if (!await _challengeHandler.UpdateStats(false, false, move.ChallengeId))
                    {
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    return Ok("You lost on time");
                case ValidationResult.OtherPlayersTurn:
                    _logger.LogWarning("User tried to move before it was his or her turn.");
                    return BadRequest("Please wait for your turn");
                case ValidationResult.WrongPlayer:
                    _logger.LogWarning("User tried to make a move in a game he or she is not playing.");
                    return BadRequest("Your are not one of the players in this game");
                case ValidationResult.WrongColor:
                    _logger.LogWarning("User sent wrong color information.");
                    return BadRequest("Move color is not the same as your color in the game");
                case ValidationResult.InvalidPromotion:
                    _logger.LogWarning("User tried to make an illegal pawn promotion.");
                    return BadRequest("You can only promote a pawn that goes to the last rank");
                case ValidationResult.NotEnoughPromotionInformation:
                    _logger.LogWarning("User tried to promote a pawn without providing info about piece to promote to.");
                    return BadRequest("You must promote your pawn to another piece when it reaches the last rank");
                case ValidationResult.GameOver:
                    return BadRequest("This game is over");
                case ValidationResult.Failed:
                    _logger.LogWarning("user tried to make an illegal move.");
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
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    _logger.LogInformation("User won the game.");
                    return Ok("You won!");
                case ValidationResult.BlackWon:
                    if (!_challengeHandler.UpdateGame(move) || !await _challengeHandler.UpdateStats(false, false, move.ChallengeId))
                    {
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    _logger.LogInformation("User won the game.");
                    return Ok("You won!");

            }
            _logger.LogInformation("User made a valid move.");
            return Ok("Move accepted");
        }

        [HttpPost("draw/request")]
        [Authorize]
        public IActionResult RequestDraw([FromBody] DrawRequestDto drawRequest)
        {
            if (!_challengeHandler.ChallengeExists(drawRequest.ChallengeId))
            {
                _logger.LogWarning("User tried to request a draw in a game that does not exist.");
                return NotFound("Game does not exist");
            }

            if (_challengeHandler.DrawRequestExists(drawRequest.ChallengeId))
            {
                _logger.LogWarning("User tried to request a draw in a game in which a draw has been requested before.");
                return BadRequest("You cannot create a second draw request for the same challenge.");
            }

            drawRequest.PlayerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (_challengeHandler.ValidateDrawRequest(drawRequest.ChallengeId, drawRequest.PlayerId))
            {
                case ValidationResult.Failed:
                    _logger.LogWarning("User tried to request a draw in a game that has not been started or a game which was finished.");
                    return BadRequest("You can only request a move in an ongoing game");
                    case ValidationResult.WrongPlayer:
                    _logger.LogWarning("User tried to request a draw in a game he or she is not playing.");
                    return BadRequest("You must be playing the game in which you request a draw");
                    case ValidationResult.Success:
                    if (!_challengeHandler.MakeDrawRequest(drawRequest))
                    {
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
            }
            _logger.LogInformation("User requested a draw.");
            return Ok("Draw request made");
        }

        [HttpPost("draw/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptDraw([FromBody] DrawAcceptDto drawAcceptDto)
        {
            if (!_challengeHandler.ChallengeExists(drawAcceptDto.ChallengeId))
            {
                _logger.LogWarning("User tried to accept a draw in game that does not exist.");
                return NotFound("Game does not exist");
            }

            if (!_challengeHandler.DrawRequestExists(drawAcceptDto.ChallengeId))
            {
                _logger.LogWarning("User tried to accept a draw that was not requested.");
                return NotFound("No draw request has been made for this game");
            }

            if (_challengeHandler.GameIsDrawn(drawAcceptDto.ChallengeId))
            {
                _logger.LogWarning("User tried to accept a draw request for a game that is already drawn.");
                return BadRequest("This game is already drawn");
            }

            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            switch (_challengeHandler.ValidateDrawAccept(drawAcceptDto.ChallengeId, playerId))
            {
                case ValidationResult.WrongPlayer:
                    _logger.LogWarning("User tried to accept a draw in a game he or she is not playing.");
                    return BadRequest("You cannot accept a draw request in a game you are not playing");
                    case ValidationResult.Failed:
                    _logger.LogWarning("User tried to accept own draw request.");
                    return BadRequest("You cannot accept your own draw request");
                    case ValidationResult.Success:
                    if (!await _challengeHandler.UpdateStats(false, true, drawAcceptDto.ChallengeId))
                    {
                        _logger.LogError("Error occured while saving to the database.");
                        return StatusCode(500, "A problem happened while handling your request.");
                    }
                    break;
            }
            _logger.LogInformation("User accepted draw request.");
            return Ok("Draw has been accepted");
        }
    }   
}
