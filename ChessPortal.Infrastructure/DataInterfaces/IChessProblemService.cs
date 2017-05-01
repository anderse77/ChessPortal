using ChessPortal.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace ChessPortal.Infrastructure.DataInterfaces
{
    public interface IChessProblemService
    {
        Task<ChessProblemResponseDto> GetChessProblemAsync(ChessProblemRequestDto request);
    }
}
