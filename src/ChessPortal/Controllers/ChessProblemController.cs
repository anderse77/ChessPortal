using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChessPortal.Web.Controllers
{
    [Route("api/problem")]
    public class ChessProblemController : Controller
    {
        private readonly IChessProblemHandler _chessProblemHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChessProblemController> _logger;

        public ChessProblemController(
            IChessProblemHandler chessProblemHandler,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChessProblemController> logger)
        {
            _chessProblemHandler = chessProblemHandler;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChessProblem()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var position = await _chessProblemHandler.GetChessProblemPositionForPlayer(playerId);
            if (position == "Error: Save failed")
            {
                _logger.LogError("Error while saving to the database.");
            }
            _logger.LogInformation("Chess problem fetched and saved to database.");
            return Ok(position);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakeMove([FromBody] MoveDto move)
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!_chessProblemHandler.ChessProblemExistsForPlayer(playerId))
            {
                _logger.LogWarning("This endpoint was called before the user actually fetched a problem");
                return BadRequest("Retrieve a problem to solve first");
            }
            switch (await _chessProblemHandler.TryMove(move, playerId))
            {
                case TryMoveResult.Error:
                    _logger.LogError("Error occured while saving to the database.");
                    return StatusCode(500, "A problem happened while handling your request.");
                case TryMoveResult.Failed:
                    _logger.LogInformation("User failed to solve the problem.");
                    return Ok("Incorrect move. You failed to solve the problem.");
                    case TryMoveResult.InvalidMove:
                        _logger.LogInformation("User made an invlaid move while trying to solve chess problem");
                        return Ok("Invalid move. You may try again");
                case TryMoveResult.SuccessProblemNotSolved:
                    _logger.LogInformation("User made correct move in the problem.");
                    return Ok(await _chessProblemHandler.GetChessProblemPositionForPlayer(playerId));
                case TryMoveResult.SuccessProblemSolved:
                    break;
            }
            _logger.LogInformation("User solve the problem.");
            return Ok("problem solved");
        }
    }
}
