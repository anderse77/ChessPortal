using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
{
    public class EnPassantInfo
    {
        public bool PawnhasJustMovedTwoSteps { get; set; }
        public int XCoordinateOfPawn { get; set; }
        public int YCoordinateOfPawn { get; set; }

        public EnPassantInfo(bool pawnHasJustMovedTwoSteps, int x, int y)
        {
            PawnhasJustMovedTwoSteps = pawnHasJustMovedTwoSteps;
            XCoordinateOfPawn = x;
            YCoordinateOfPawn = y;
        }

        public EnPassantInfo(bool pawnHasJustMovedTwoSteps) : this(false, 0, 0)
        {
            
        }
    }
}
