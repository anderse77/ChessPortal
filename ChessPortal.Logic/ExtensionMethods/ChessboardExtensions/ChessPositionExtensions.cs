using ChessPortal.Logic.Chess;

namespace ChessPortal.Logic.ExtensionMethods.ChessboardExtensions
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
