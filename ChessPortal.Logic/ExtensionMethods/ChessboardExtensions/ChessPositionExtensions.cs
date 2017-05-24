using ChessPortal.Logic.Chess;

namespace ChessPortal.Logic.ExtensionMethods.ChessboardExtensions
{
    public static class ChessPositionExtensions
    {
        public static bool ContentEquals(this ChessPosition board, ChessPosition other)
        {
            for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                for (int j = 0; j < BoardCharacteristics.SideLength; j++)
                    if (!board[j, i].Equals(other[j, i]))
                    {
                        return false;
                    }
            return true;
        }
    }
}
