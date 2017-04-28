using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Entities;
using ChessPortal.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChessPortal.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ChessPlayer> _userManager;
        private readonly SignInManager<ChessPlayer> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ChessPlayer> userManager,
            SignInManager<ChessPlayer> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new ChessPlayer { UserName = registerDto.UserName, Email = registerDto.Email };
                var result = await _userManager.CreateAsync(user, registerDto.Password);
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
    }
}
