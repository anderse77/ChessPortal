using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.ExtensionMethods.ChessboardExtensions;

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

        public IList<Move> GetValidMoveCandidates()
        {
            IList<Move> validMoves = new List<Move>();
            var position = History.Last();
            for (int i = 0; i < position.GetLength(1); i++)
            {
                for (int j = 0; j < position.GetLength(0); j++)
                {
                    var square = position[j, i];
                    if (square.Piece.HasValue && square.Color.HasValue && IsOwnColor(square.Color.Value))
                    {
                        if (square.Piece == Piece.Pawn && i == (square.Color == Color.White ? 1 : 7))
                        {
                            var direction = RuleInfo.PawnMoveDirection[square.Color.Value];
                            var yModifier = direction == Direction.North ? 2 : -2;
                            if (!position[j, i + yModifier].Piece.HasValue)
                            {
                                validMoves.Add(new Move(Piece.Pawn, j, j, i, i + yModifier, square.Color.Value));
                            }
                        }
                        else if (square.Piece == Piece.Pawn && i == 4)
                        {
                            foreach (Direction direction in RuleInfo.PawnCaptureDirections[square.Color.Value])
                            {
                                var modifiers = direction.GetModifiers();
                                var move = new Move(Piece.Pawn, j, j + modifiers[0], i, i + modifiers[1], square.Color.Value);
                                if (move.PawnCanCaptureEnPassant(position, MoveList))
                                {
                                    move.IsEnPassant = true;
                                    validMoves.Add(move);
                                }
                            }
                        }
                        if (square.Piece == Piece.Pawn)
                        {
                            foreach (Direction direction in RuleInfo.PawnCaptureDirections[square.Color.Value])
                            {
                                var modifiers = direction.GetModifiers();
                                var captureSquare = position[j + modifiers[0], i + modifiers[1]];
                                if (captureSquare.Color.HasValue && !IsOwnColor(captureSquare.Color.Value))
                                {
                                    validMoves.Add(new Move(Piece.Pawn, j, j + modifiers[0], i, i + modifiers[1], square.Color.Value)
                                    {
                                        IsCapture = true
                                    });
                                }
                            }

                            var pawnMoveDirection = RuleInfo.PawnMoveDirection[square.Color.Value];
                            var yModifier = pawnMoveDirection == Direction.North ? 1 : -1;
                            if (!position[j, i + yModifier].Piece.HasValue)
                            {
                                validMoves.Add(new Move(Piece.Pawn, j, j, i, i + yModifier, square.Color.Value));
                            }


                        }
                        if (square.Piece == Piece.King && j == 4 && i == (square.Color == Color.White ? 0 : 7))
                        {
                            var move1 = new Move(Piece.King, j, j + 2, i, i, square.Color.Value);
                            var move2 = new Move(Piece.King, j, j - 2, i, i, square.Color.Value);
                            if (move1.KingCanCastle(MoveList, History.Last()))
                            {
                                move1.IsCastle = true;
                                validMoves.Add(move1);
                            }
                            if (move2.KingCanCastle(MoveList, History.Last()))
                            {
                                move2.IsCastle = true;
                                validMoves.Add(move2);
                            }
                        }
                        if (square.Piece != Piece.Pawn)
                        {
                            foreach (Direction direction in RuleInfo.ValidDirections[square.Piece.Value])
                            {
                                var modifiers = direction.GetModifiers();
                                int length = 0;
                                while (j + modifiers[0] > 0 && j + modifiers[0] < 7 && i + modifiers[1] > 0 &&
                                       i + modifiers[1] < 7 &&
                                       !position[j + modifiers[0], i + modifiers[1]].Color.HasValue &&
                                       length < RuleInfo.MaximumMoveLength[square.Piece.Value])
                                {
                                    validMoves.Add(new Move(square.Piece.Value, j, j + length++ * modifiers[0], i, i + length++ * modifiers[1], square.Color.Value));
                                }

                                if (j + modifiers[0] > 0 && j + modifiers[0] < 7 && i + modifiers[1] > 0 &&
                                    i + modifiers[1] < 7 && length >= RuleInfo.MaximumMoveLength[square.Piece.Value] &&
                                    position[j + modifiers[0], i + modifiers[1]].Color.HasValue &&
                                    !IsOwnColor(position[j + modifiers[0], i + modifiers[1]].Color.Value))
                                {
                                    validMoves.Add(new Move(square.Piece.Value, j, j + length++ * modifiers[0], i, i + length++ * modifiers[1], square.Color.Value)
                                    {
                                        IsCapture = true
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return validMoves;
        }

        bool IsOwnColor(Color color)
        {
            return color == (History.Last().WhiteToMove ? Color.White : Color.Black);
        }

        public bool MakeMove(Move move)
        {
            if (GameStatus == GameStatus.Ongoing)
            {
                var validMoveCandidates = GetValidMoveCandidates();
                var detailedMove = validMoveCandidates.SingleOrDefault(m => m == move);

                if (detailedMove == null)
                {
                    return false;
                }

                return UpdateGame(move);
            }
            return false;
        }

        bool UpdateGame(Move move)
        {
            var oldPosition = History.Last();
            var newPosition = oldPosition.UpdateBoard(move);
            if (!newPosition.ContentEquals(oldPosition))
            {
                History.Add(newPosition);

                MoveList.Add(move);

                if (IsCheckMate())
                {
                    GameStatus = newPosition.WhiteToMove ? GameStatus.BlackWins : GameStatus.WhiteWins;
                }

                if (IsDrawByStalemate() && IsDrawByThreeFoldRepetition() || IsDrawByFiftyMoveRule())
                {
                    GameStatus = GameStatus.Draw;
                }

                return true;
            }
            return false;
        }

        bool IsCheckMate()
        {
            return History.Last().OwnKingIsLeftInCheck() &&
                   GetValidMoveCandidates().All(m => History.Last().UpdateBoard(m).OwnKingIsLeftInCheck());
        }

        bool IsDrawByStalemate()
        {
            return !History.Last().OwnKingIsLeftInCheck() &&
                   GetValidMoveCandidates().All(m => History.Last().UpdateBoard(m).OwnKingIsLeftInCheck());
        }

        bool IsDrawByThreeFoldRepetition()
        {
            var positionHashSet = new HashSet<ChessPosition>();
            foreach (var position in History)
            {
                positionHashSet.Add(position);
            }
            return History.Count - positionHashSet.Count > 2;
        }

        bool IsDrawByFiftyMoveRule()
        {
            return MoveList.Reverse().Take(50).All(m => !m.IsPawnMove && !m.IsCapture);
        }
    }
}
