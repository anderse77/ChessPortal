using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess
{
    public enum GameStatus
    {
        WhiteWins,
        BlackWins,
        Draw,
        NotStarted,
        Ongoing

    }
    public class ChessGame
    {
        public IList<ChessPosition> History { get; }

        public IList<Move> MoveList { get; }

        public int DaysPerMove { get; }

        public GameStatus GameStatus { get; set; }

        public ChessGame(int daysPerMove)
        {
            History = new List<ChessPosition>
            {
                new ChessPosition(new[,]
                {
                    {
                        new Square(Piece.Rook, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Rook, Color.Black)
                    },
                    {
                        new Square(Piece.Knight, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Knight, Color.Black)
                    },
                    {
                        new Square(Piece.Bishop, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Bishop, Color.Black)
                    },
                    {
                        new Square(Piece.Queen, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Queen, Color.Black)
                    },
                    {
                        new Square(Piece.King, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.King, Color.Black)
                    },
                    {
                        new Square(Piece.Bishop, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Bishop, Color.Black)
                    },
                    {
                        new Square(Piece.Knight, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Knight, Color.Black)
                    },
                    {
                        new Square(Piece.Rook, Color.White), new Square(Piece.Pawn, Color.White),
                        new Square(), new Square(),
                        new Square(), new Square(),
                        new Square(Piece.Pawn, Color.Black), new Square(Piece.Rook, Color.Black)
                    }
                }, true)
            };

            MoveList = new List<Move>();
            DaysPerMove = daysPerMove;
            GameStatus = GameStatus.NotStarted;
        }

        public bool MakeMove(Move move)
        {
            if (move.CoordinatesAreValid() && move.HasValidDirection())
            {
                if (move.Piece == Piece.Pawn && MakePawnCapture(move))
                {
                    return true;
                }
            }
        }

        bool MakePawnCapture(Move move)
        {
            if (move.PawnCanCapture(History.Last())
            {
                
            }

        }

        bool UpdateGame(Move move)
        {
            var newPosition = 
        }
    }
}
