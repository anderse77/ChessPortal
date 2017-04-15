using ChessPortal.DataInterfaces;
using ChessPortal.Handlers;
using ChessPortal.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChessPortal.Controllers
{
    [Route("api/problem")]
    public class ChessProblemController : Controller
    {
        private readonly IChessProblemHandler _chessProblemHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChessProblemController(
            IChessProblemHandler chessProblemHandler,
            IHttpContextAccessor httpContextAccessor)
        {
            _chessProblemHandler = chessProblemHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChessProblem()
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(await _chessProblemHandler.GetChessProblemPositionForPlayer(playerId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakeMove([FromBody] MoveDto move)
        {
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!_chessProblemHandler.ChessProblemExistsForPlayer(playerId))
            {
                return BadRequest("Retrieve a problem to solve first");
            }
            switch (await _chessProblemHandler.TryMove(move, playerId))
            {
                case TryMoveResult.Error:
                    return StatusCode(500, "A problem happened while handling your request.");
                case TryMoveResult.Failed:
                    return Ok("Incorrect move. You failed to solve the problem");
                case TryMoveResult.SuccessProblemNotSolved:
                    return Ok(await _chessProblemHandler.GetChessProblemPositionForPlayer(playerId));
                case TryMoveResult.SuccessProblemSolved:
                    break;
            }
            return Ok("problem solved");
        }
    }
}
