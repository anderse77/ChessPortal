using ChessPortal.Models.Chess.ChessProblems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.DataInterfaces
{
    public interface IChessProblemService
    {
        Task<ChessProblemResponse> GetChessProblemAsync(ChessProblemRequest request);
    }
}
