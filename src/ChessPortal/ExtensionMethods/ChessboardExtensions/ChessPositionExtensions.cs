using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Models.Chess;

namespace ChessPortal.ExtensionMethods.ChessboardExtensions
{
    public static class ChessPositionExtensions
    {
        public static bool ContentEquals(this ChessPosition board, ChessPosition other)
        {
            for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                for (int j = 0; j < BoardCharacteristics.SideLength; j++)
                    if (!board[i, j].Equals(other[i, j]))
                    {
                        return false;
                    }
            return true;
        }
    }
}
