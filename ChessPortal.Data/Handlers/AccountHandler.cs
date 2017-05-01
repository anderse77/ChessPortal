using ChessPortal.Data.Entities;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ChessPortal.Data.Handlers
{
    public class AccountHandler : IAccountHandler
    {
        private readonly UserManager<ChessPlayer> _userManager;

        public AccountHandler(UserManager<ChessPlayer> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAccountAsync(RegisterDto registerDto)
        {
            var user = new ChessPlayer { UserName = registerDto.UserName, Email = registerDto.Email };
            return await _userManager.CreateAsync(user, registerDto.Password);
        }
    }
}
