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

    public enum AlgebraicNotationLetters
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
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

        IList<Move> GetValidMoveCandidates()
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
                        if (square.Piece == Piece.Pawn && i == (square.Color == Color.White ? 1 : 6))
                        {
                            var direction = RuleInfo.PawnMoveDirection[square.Color.Value];
                            var yModifier = direction == Direction.North ? 2 : -2;
                            if (position.GetNearestOccupiedSquare(j, i, direction, 2).Square.Piece == null)
                            {
                                AddMoveToListIfValid(validMoves,
                                    new Move(Piece.Pawn, j, j, i, i + yModifier, square.Color.Value, null), position);

                            }
                        }
                        else if (square.Piece == Piece.Pawn && i == (square.Color == Color.White ? 4 : 3))
                        {
                            foreach (Direction direction in RuleInfo.PawnCaptureDirections[square.Color.Value])
                            {
                                var modifiers = direction.GetModifiers();
                                var move = new Move(Piece.Pawn, j, j + modifiers[0], i, i + modifiers[1], square.Color.Value, null);
                                if (move.PawnCanCaptureEnPassant(position, MoveList))
                                {
                                    move.IsEnPassant = true;
                                    AddMoveToListIfValid(validMoves, move, position);
                                }
                            }
                        }
                        if (square.Piece == Piece.Pawn)
                        {
                            foreach (Direction direction in RuleInfo.PawnCaptureDirections[square.Color.Value])
                            {
                                var modifiers = direction.GetModifiers();
                                if (IsValidChessBoardCoordinates(j + modifiers[0], i + modifiers[1]) &&
                                    position[j + modifiers[0], i + modifiers[1]].Color.HasValue &&
                                    !IsOwnColor(position[j + modifiers[0], i + modifiers[1]].Color.Value))
                                {
                                    AddMoveToListIfValid(validMoves,
                                        new Move(Piece.Pawn, j, j + modifiers[0], i, i + modifiers[1],
                                            square.Color.Value, null)
                                        {
                                            IsCapture = true
                                        }, position);
                                }
                            }

                            var pawnMoveDirection = RuleInfo.PawnMoveDirection[square.Color.Value];
                            var yModifier = pawnMoveDirection == Direction.North ? 1 : -1;
                            if (!position[j, i + yModifier].Piece.HasValue)
                            {
                                var move = new Move(Piece.Pawn, j, j, i, i + yModifier, square.Color.Value, null);
                                if (!position.UpdateBoardIfValid(move).ContentEquals(position))
                                {
                                    validMoves.Add(new Move(Piece.Pawn, j, j, i, i + yModifier, square.Color.Value, null));
                                }
                            }


                        }
                        if (square.Piece == Piece.King && j == 4 && i == (square.Color == Color.White ? 0 : 7))
                        {
                            var move1 = new Move(Piece.King, j, j + 2, i, i, square.Color.Value, null);
                            var move2 = new Move(Piece.King, j, j - 2, i, i, square.Color.Value, null);
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
                                int length = 1;
                                while (IsValidChessBoardCoordinates(j + length * modifiers[0], i + length * modifiers[1]) &&
                                       length <= RuleInfo.MaximumMoveLength[square.Piece.Value] &&
                                       !position[j + length * modifiers[0], i + length * modifiers[1]].Color.HasValue)
                                {
                                    AddMoveToListIfValid(validMoves,
                                        new Move(square.Piece.Value, j, j + length * modifiers[0], i,
                                            i + length * modifiers[1], square.Color.Value, null), position);
                                    length++;
                                }

                                if (IsValidChessBoardCoordinates(j + length * modifiers[0], i + length * modifiers[1]) && length <= RuleInfo.MaximumMoveLength[square.Piece.Value] &&
                                    position[j + length * modifiers[0], i + length * modifiers[1]].Color.HasValue &&
                                    !IsOwnColor(position[j + length * modifiers[0], i + length * modifiers[1]].Color.Value))
                                {
                                    AddMoveToListIfValid(validMoves,
                                        new Move(square.Piece.Value, j, j + length * modifiers[0], i,
                                            i + length * modifiers[1], square.Color.Value, null)
                                        {
                                            IsCapture = true
                                        }, position);

                                }
                            }
                        }
                    }
                }
            }
            return validMoves;
        }

        void AddMoveToListIfValid(IList<Move> moves, Move move, ChessPosition position)
        {
            if (!position.UpdateBoardIfValid(move).ContentEquals(position))
            {
                moves.Add(move);
            }
        }

        bool IsValidChessBoardCoordinates(int x, int y)
        {
            return x > -1 && x < 8 && y > -1 && y < 8;
        }

        bool IsOwnColor(Color color)
        {
            return color == (History.Last().WhiteToMove ? Color.White : Color.Black);
        }

        public bool MakeMove(Move move)
        {
            if (GameStatus == GameStatus.NotStarted || GameStatus == GameStatus.Ongoing)
            {
                var validMoveCandidates = GetValidMoveCandidates();
                var detailedMove = validMoveCandidates.SingleOrDefault(m => m == move);                
                if (detailedMove == null)
                {
                    return false;
                }
                detailedMove.PromoteTo = move.PromoteTo;
                return UpdateGame(detailedMove);
            }
            return false;
        }

        public bool UpdateGame(Move move)
        {
            var oldPosition = History.Last();
            var newPosition = oldPosition.UpdateBoardIfValid(move);
            if (!newPosition.ContentEquals(oldPosition))
            {
                History.Add(newPosition);

                MoveList.Add(move);

                if (GameStatus == GameStatus.NotStarted)
                {
                    GameStatus = GameStatus.Ongoing;
                }

                if (IsCheckMate(move.Color == Color.White ? Color.Black : Color.White))
                {
                    GameStatus = newPosition.WhiteToMove ? GameStatus.BlackWins : GameStatus.WhiteWins;
                }

                if (IsDrawByStalemate(move.Color == Color.White ? Color.Black : Color.White) && IsDrawByThreeFoldRepetition() || IsDrawByFiftyMoveRule() || IsDrawByInsufficientMaterial())
                {
                    GameStatus = GameStatus.Draw;
                }

                return true;
            }
            return false;
        }

        public bool UpdateGame(string move)
        {
            var position = History.Last();
            int toX = 0;
            int toY = 0;
            var color = position.WhiteToMove ? Color.White : Color.Black;
            //move is castle
            if (move == "O-O" || move == "O-O-O")
            {
                var fromX = 4;
                var fromY = position.WhiteToMove ? 0 : 7;
                toX = move == "O-O" ? 6 : 2;
                toY = fromY;
                UpdateGame(new Move(Piece.King, fromX, toX, fromY, toY, color, null)
                {
                    IsCastle = true
                });
            }
            bool isCapture = false;
            //check if move is capture and remove corresponding character.
            if (move.Any(c => c == 'x'))
            {
                isCapture = true;
                move = move.Replace("x", "");
            }
            //remove any + sign indicating that the move is check
            if (move.Any(c => c == '+'))
            {
                move = move.Replace("+", "");
            }
            try
            {
                toX = (int)Enum.Parse(typeof(AlgebraicNotationLetters), move[move.Length - 2].ToString());
                toY = int.Parse(move[move.Length - 1].ToString());
            }
            catch
            {
                throw new ArgumentException("move is not in algebraic notation format");
            }
            //move is pawn capture
            if (move.First().ToString().ToLower() == move.First().ToString())
            {
                Piece? promoteTo = null;
                if (move.Any(c => c == '='))
                {
                    promoteTo = Square.FromFenChar(move.Last()).Piece;
                    move = move.Substring(0, move.Length - 2);
                }
                int fromX = 0;
                int fromY = 0;
                var validMoveCandidates = GetValidMoveCandidates();
                try
                {
                    fromX = (int)Enum.Parse(typeof(AlgebraicNotationLetters), move.First().ToString());
                }
                catch
                {
                    throw new ArgumentException("move is not in algebraic notation format");
                }
                for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                {
                    var square = position[fromX, i];
                    if (square.Color.HasValue && square.Color == color && square.Piece.HasValue && square.Piece.Value == Piece.Pawn)
                    {
                        fromY = i;
                        var moveCandidate = new Move(Piece.Pawn, fromX, toX, fromY, toY, color, promoteTo)
                        {
                            IsCapture = isCapture
                        };
                        if (validMoveCandidates.Contains(moveCandidate))
                        {
                            return UpdateGame(moveCandidate);
                        }
                    }
                }
            }
            var piece = Square.FromFenChar(move[0]).Piece.Value;            
            if (move.Length == 4)
            {
                var rankOrFileIdentifier = move[1];
                int number = 0;
                if (int.TryParse(rankOrFileIdentifier.ToString(), out number))
                {
                    for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                    {
                        var square = position[i, number - 1];
                        if (square.Color.HasValue && square.Color == color && square.Piece.HasValue && square.Piece.Value == piece)
                        {
                            var moveCandidate = new Move(piece, i, toX, number - 1, toY, square.Color.Value, null)
                            {
                                IsCapture = isCapture
                            };
                            return UpdateGame(moveCandidate);
                        }
                    }
                    return false;
                }
                else
                {
                    var fileNumber = (int)Enum.Parse(typeof(AlgebraicNotationLetters), rankOrFileIdentifier.ToString());
                    for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                    {

                        var square = position[fileNumber, i];
                        if (square.Color.HasValue && square.Color == color && square.Piece.HasValue && square.Piece.Value == piece)
                        {
                            var moveCandidate = new Move(piece, fileNumber, toX, i, toY, square.Color.Value, null)
                            {
                                IsCapture = isCapture
                            };
                            return UpdateGame(moveCandidate);
                        }
                    }
                }
            }
            else if (move.Length == 3)
            {
                for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                {
                    for (int j = 0; j < BoardCharacteristics.SideLength; j++)
                    {
                        var square = position[j, i];
                        if (square.Color.HasValue && square.Color == color && square.Piece.HasValue && square.Piece.Value == piece)
                        {
                            var moveCandidate = new Move(piece, j, toX, i, toY, color, null)
                            {
                                IsCapture = isCapture
                            };
                            var validMoveCandidates = GetValidMoveCandidates();
                            if (validMoveCandidates.Contains(moveCandidate))
                            {
                                return UpdateGame(moveCandidate);
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool IsCheckMate(Color color)
        {
            return History.Last().KingIsInCheck(color) &&
                   !GetValidMoveCandidates().Any();
        }

        bool IsDrawByStalemate(Color color)
        {
            return !History.Last().KingIsInCheck(color) &&
                   !GetValidMoveCandidates().Any();
        }

        bool IsDrawByThreeFoldRepetition()
        {
            var positionHashSet = new HashSet<ChessPosition>();
            foreach (var position in History)
            {
                positionHashSet.Add(position);
            }
            return History.Count - positionHashSet.Count >= 2;
        }

        bool IsDrawByFiftyMoveRule()
        {
            return MoveList.Reverse().Take(50).All(m => !m.IsPawnMove && !m.IsCapture);
        }

        bool IsDrawByInsufficientMaterial()
        {
            var position = History.Last();
            if (position.All(s => !s.Piece.HasValue || s.Piece.Value != Piece.Pawn))
            {
                var numberOfWhitePieces = position.Count(s => s.Color.HasValue && s.Color.Value == Color.White);
                var numberOfBlackPieces = position.Count(s => s.Color.HasValue && s.Color == Color.Black);
                if (numberOfWhitePieces == 1 || numberOfBlackPieces == 1)
                {
                    var colorOfPlayerWithMorePieces = numberOfWhitePieces == 1 ? Color.Black : Color.White;
                    var highestNumberOfPieces = Math.Max(numberOfWhitePieces, numberOfBlackPieces);
                    if (highestNumberOfPieces == 2 &&
                        position.All(
                            s => !s.Piece.HasValue || (s.Piece.Value != Piece.Rook && s.Piece.Value != Piece.Queen)))
                    {
                        return true;
                    }

                    if (highestNumberOfPieces == 3 &&
                        position.All(
                            s =>
                                !s.Piece.HasValue ||
                                (s.Piece.Value != Piece.Bishop && s.Piece.Value != Piece.Rook &&
                                 s.Piece.Value != Piece.Queen)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
