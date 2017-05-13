using ChessPortal.Data.Entities;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChessPortal.Infrastructure.DataInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChessPortal.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ChessPlayer> _userManager;
        private readonly SignInManager<ChessPlayer> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountHandler _accountHandler;
        private readonly IChessPlayerDtoProvider _chessPlayerDtoProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(UserManager<ChessPlayer> userManager,
            SignInManager<ChessPlayer> signInManager,
            ILogger<AccountController> logger,
            IAccountHandler accountHandler,
            IChessPlayerDtoProvider chessPlayerDtoProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _accountHandler = accountHandler;
            _chessPlayerDtoProvider = chessPlayerDtoProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountHandler.CreateAccountAsync(registerDto);
                if (result.Succeeded)
                {
                    _logger.LogInformation(3, "User created a new account with password.");
                    return Ok();
                }

                _logger.LogError("Failed to create account");
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false,
                    false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {loginDto.UserName} logged in.");
                    return Ok();
                }

                var errorMessage = "Invalid login attempt";

                if (result.RequiresTwoFactor)
                {
                    errorMessage = "Two factor required";
                }
                if (result.IsLockedOut)
                {
                    errorMessage = "User account locked out.";
                }
                if (result.IsNotAllowed)
                {
                    errorMessage = "User is not allowed to log in";
                }

                _logger.LogWarning(errorMessage);

                return BadRequest(errorMessage);
            }
            _logger.LogWarning("Model state is invalid");

            return BadRequest(ModelState);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok();
        }

        [HttpGet("player")]
        [Authorize]
        public IActionResult GetCurrentPlayerStats()
        {
            _logger.LogInformation("User fetches player stats.");
            var playerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(_chessPlayerDtoProvider.GetPlayer(playerId));
        }
    }
}
