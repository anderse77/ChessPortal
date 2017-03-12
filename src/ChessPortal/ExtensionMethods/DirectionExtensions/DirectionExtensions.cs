using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.Models.Chess;

namespace ChessPortal.ExtensionMethods.DirectionExtensions
{
    public static class DirectionExtensions
    {
        public static int[] GetStep(this Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return new[] {1, 0};
                case Direction.Eastnortheast:
                    return new[] {2, 1};
                case Direction.Eastsoutheast:
                    return new[] {2, -1};
                case Direction.North:
                    return new[] {0, 1};
                case Direction.Southsouthwest:
                    return new[] {-1, -2};
                case Direction.Northeast:
                    return new[] {1, 1};
                case Direction.Northnorthwest:
                    return new[] {-1, 2};
                case Direction.Northnortheast:
                    return new[] {1, 2};
                case Direction.South:
                    return new[] {0, -1};
                case Direction.West:
                    return new[] {-1, 0};
                case Direction.Northwest:
                    return new[] {-1, 1};
                case Direction.Westsouthwest:
                    return new[] {-2, -1};
                case Direction.Southeast:
                    return new[] {1, -1};
                case Direction.Southwest:
                    return new[] {-1, -1};
                case Direction.Southsoutheast:
                    return new[] {1, -2};
                case Direction.Westnorthwest:
                    return new[] {-2, 1};
                default:
                    return new[] {0, 0};
            }
        }
    }
}
