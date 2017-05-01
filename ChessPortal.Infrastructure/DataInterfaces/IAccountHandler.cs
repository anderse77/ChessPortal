using ChessPortal.Infrastructure.Dtos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IAccountHandler
    {
        Task<IdentityResult> CreateAccountAsync(RegisterDto registerDto);
    }
}
