using ChessPortal.Logic.ExtensionMethods.ChessboardExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPortal.Logic.Chess
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

        public ChessGame(ChessPosition position)
        {
            History = new List<ChessPosition>
            {
                position
            };
            MoveList = new List<Move>();
            GameStatus = GameStatus.Ongoing;
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

                SetGameStatus(move, newPosition);

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
            var validMoveCandidates = GetValidMoveCandidates();
            if (IsCastle(move))
            {
                return Castle(move, position, color);
            }
            bool isCapture = false;
            if (IsCapture(move))
            {
                isCapture = true;
                move = move.Replace("x", "");
            }
            if (IsCheck(move))
            {
                move = move.Replace("+", "");
            }
            if (IsCheckMate(move))
            {
                move = move.Replace("#", "");
            }
            try
            {
                toX = (int)Enum.Parse(typeof(AlgebraicNotationLetters), move[move.Length - 2].ToString());
                toY = int.Parse(move[move.Length - 1].ToString()) - 1;
            }
            catch
            {
                throw new ArgumentException("move is not in algebraic notation format");
            }
            if (IsPawnMove(move))
            {
                Piece? promoteTo = null;
                if (IsPawnPromotion(move))
                {
                    promoteTo = Square.FromFenChar(move.Last()).Piece;
                    move = move.Substring(0, move.Length - 2);
                }
                return ProcessPawnMove(move, position, toX, toY, color, promoteTo, isCapture, validMoveCandidates);
            }
            var piece = Square.FromFenChar(move[0]).Piece.Value;
            return ProcessNonPawnMove(move, position, toX, toY, color, piece, isCapture, validMoveCandidates);
        }

        bool ProcessNonPawnMove(string move, ChessPosition position, int toX, int toY, Color color, Piece piece, bool isCapture, IList<Move> validMoveCandidates)
        {
            if (move.Length == 4)
            {
                var rankOrFileIdentifier = move[1];
                int number = 0;
                if (int.TryParse(rankOrFileIdentifier.ToString(), out number))
                {
                    for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                    {
                        var square = position[i, number - 1];
                        var fromX = i;
                        var fromY = number - 1;
                        if (UpdateGameIfPieceFound(square, fromX, fromY, toX, toY, color, piece, isCapture,
                            validMoveCandidates))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    number = (int)Enum.Parse(typeof(AlgebraicNotationLetters), rankOrFileIdentifier.ToString());
                    for (int i = 0; i < BoardCharacteristics.SideLength; i++)
                    {
                        var fromX = number;
                        var fromY = i;
                        var square = position[number, i];
                        if (UpdateGameIfPieceFound(square, fromX, fromY, toX, toY, color, piece, isCapture,
                            validMoveCandidates))
                        {
                            return true;
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
                        var fromX = j;
                        var fromY = i;
                        if (UpdateGameIfPieceFound(square, fromX, fromY, toX, toY, color, piece, isCapture, validMoveCandidates))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        bool UpdateGameIfPieceFound(Square square, int fromX, int fromY, int toX, int toY, Color color, Piece piece, bool isCapture, IList<Move> validMoveCandidates)
        {
            if (square.Color.HasValue && square.Color == color && square.Piece.HasValue && square.Piece.Value == piece)
            {
                var moveCandidate = new Move(piece, fromX, toX, fromY, toY, color, null)
                {
                    IsCapture = isCapture
                };
                if (validMoveCandidates.Contains(moveCandidate))
                {
                    return UpdateGame(moveCandidate);
                }
            }
            return false;
        }

        bool ProcessPawnMove(string move, ChessPosition position, int toX, int toY, Color color, Piece? promoteTo, bool isCapture, IList<Move> validMoveCandidates)
        {
            int fromX = 0;
            int fromY = 0;
            try
            {
                fromX = (int)Enum.Parse(typeof(AlgebraicNotationLetters), move.First().ToString());
            }
            catch
            {
                throw new ArgumentException("move is not in algebraic notation format");
            }
            if (MoveIsFirstMoveInProblemAndPawnMoveIsEnpassant(position, move))
            {
                position.EnPassantCaptureLocationIfSetupFromFen = string.Empty;
                return UpdateGame(new Move(Piece.Pawn, fromX, toX, color == Color.White ? 4 : 3, toY, color, null)
                {
                    IsEnPassant = true
                });
            }
            return FindPawnAndUpdateGame(position, fromX, color, fromY, toX, toY, null, isCapture, validMoveCandidates);
        }

        bool FindPawnAndUpdateGame(ChessPosition position, int fromX, Color color, int fromY, int toX, int toY, Piece? promoteTo, bool isCapture, IList<Move> validMoveCandidates)
        {
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
            throw new ArgumentException("move is not in algebraic notation format");
        }

        bool MoveIsFirstMoveInProblemAndPawnMoveIsEnpassant(ChessPosition position, string move)
        {
            return !string.IsNullOrEmpty(position.EnPassantCaptureLocationIfSetupFromFen) &&
                    position.EnPassantCaptureLocationIfSetupFromFen == move.Substring(move.Length - 2);
        }

        bool IsPawnPromotion(string move)
        {
            return move.Any(c => c == '=');
        }

        bool IsPawnMove(string move)
        {
            return move.First().ToString().ToLower() == move.First().ToString();
        }

        bool IsCapture(string move)
        {
            return move.Any(c => c == 'x');
        }

        bool IsCheck(string move)
        {
            return move.Any(c => c == '+');
        }

        bool IsCheckMate(string move)
        {
            return move.Any(c => c == '#');
        }

        bool Castle(string move, ChessPosition position, Color color)
        {
            var fromX = 4;
            var fromY = position.WhiteToMove ? 0 : 7;
            var toX = move == "O-O" ? 6 : 2;
            var toY = fromY;
            return UpdateGame(new Move(Piece.King, fromX, toX, fromY, toY, color, null)
            {
                IsCastle = true
            });
        }

        bool IsCastle (string move)
        {
            return move == "O-O" || move == "O-O-O";
        }

        void SetGameStatus(Move move, ChessPosition newPosition)
        {
            if (GameStatus == GameStatus.NotStarted)
            {
                GameStatus = GameStatus.Ongoing;
            }

            if (IsCheckMate(move.Color == Color.White ? Color.Black : Color.White))
            {
                GameStatus = newPosition.WhiteToMove ? GameStatus.BlackWins : GameStatus.WhiteWins;
            }

            if (IsDrawByStalemate(move.Color == Color.White ? Color.Black : Color.White) || IsDrawByThreeFoldRepetition() || IsDrawByFiftyMoveRule() || IsDrawByInsufficientMaterial())
            {
                GameStatus = GameStatus.Draw;
            }
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
                        FindValidMovesForPiece(square, i, j, position, validMoves);
                    }
                }
            }
            return validMoves;
        }

        void FindValidMovesForPiece(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            
            if (square.Piece == Piece.King)
            {
                AddCastlingMovesIfValid(square, y, x, position, validMoves);
            }
            if (square.Piece == Piece.Pawn)
            {
                FindValidMovesForPawn(square, y, x, position, validMoves);
            }
            else
            {
                AddValidCommonMovesForNonPawn(square, y, x, position, validMoves);
            }
        }

        void AddValidCommonMovesForNonPawn(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            foreach (Direction direction in RuleInfo.ValidDirections[square.Piece.Value])
            {
                var modifiers = direction.GetModifiers();
                int length = 1;
                while (NonPawnCanMove(y, x, length, modifiers, square, position))
                {
                    AddMoveToListIfValid(validMoves,
                        new Move(square.Piece.Value, x, x + length * modifiers[0], y,
                            y + length * modifiers[1], square.Color.Value, null), position);
                    length++;
                }

                if (NonPawnCanCapture(y, x, length, modifiers, square, position))
                {
                    AddMoveToListIfValid(validMoves,
                        new Move(square.Piece.Value, x, x + length * modifiers[0], y,
                            y + length * modifiers[1], square.Color.Value, null)
                        {
                            IsCapture = true
                        }, position);

                }
            }
        }

        bool NonPawnCanCapture(int y, int x, int length, int[] modifiers, Square square, ChessPosition position)
        {
            return IsValidChessBoardCoordinates(x + length * modifiers[0], y + length * modifiers[1]) && length <= RuleInfo.MaximumMoveLength[square.Piece.Value] &&
                    position[x + length * modifiers[0], y + length * modifiers[1]].Color.HasValue &&
                    !IsOwnColor(position[x + length * modifiers[0], y + length * modifiers[1]].Color.Value);
        }

        bool NonPawnCanMove(int y, int x, int length, int[] modifiers, Square square, ChessPosition position)
        {
            return IsValidChessBoardCoordinates(x + length * modifiers[0], y + length * modifiers[1]) &&
                       length <= RuleInfo.MaximumMoveLength[square.Piece.Value] &&
                       !position[x + length * modifiers[0], y + length * modifiers[1]].Color.HasValue;
        }

        void AddCastlingMovesIfValid(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            if (x == 4 && y == (square.Color == Color.White ? 0 : 7))
            {
                var move1 = new Move(Piece.King, x, x + 2, y, y, square.Color.Value, null);
                var move2 = new Move(Piece.King, x, x - 2, y, y, square.Color.Value, null);
                if (move1.KingCanCastle(MoveList, position))
                {
                    move1.IsCastle = true;
                    validMoves.Add(move1);
                }
                if (move2.KingCanCastle(MoveList, position))
                {
                    move2.IsCastle = true;
                    validMoves.Add(move2);
                }
            }
        }

        void FindValidMovesForPawn(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            AddLongPawnMoveIfValid(square, y, x, position, validMoves);
            AddEnPassantIfValid(square, y, x, position, validMoves);
            AddValidCommonMovesForPawn(square, y, x, position, validMoves);
        }

        void AddValidCapturesForPawn(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            foreach (Direction direction in RuleInfo.PawnCaptureDirections[square.Color.Value])
            {
                var modifiers = direction.GetModifiers();
                if (IsValidChessBoardCoordinates(x + modifiers[0], y + modifiers[1]) &&
                    position[x + modifiers[0], y + modifiers[1]].Color.HasValue &&
                    !IsOwnColor(position[x + modifiers[0], y + modifiers[1]].Color.Value))
                {
                    AddMoveToListIfValid(validMoves,
                        new Move(Piece.Pawn, x, x + modifiers[0], y, y + modifiers[1],
                            square.Color.Value, null)
                        {
                            IsCapture = true
                        }, position);
                }
            }
        }

        void AddForwardMoveForPawnIfValid(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            var pawnMoveDirection = RuleInfo.PawnMoveDirection[square.Color.Value];
            var yModifier = pawnMoveDirection == Direction.North ? 1 : -1;
            if (!position[x, y + yModifier].Piece.HasValue)
            {
                var move = new Move(Piece.Pawn, x, x, y, y + yModifier, square.Color.Value, null);
                if (!position.UpdateBoardIfValid(move).ContentEquals(position))
                {
                    validMoves.Add(new Move(Piece.Pawn, x, x, y, y + yModifier, square.Color.Value, null));
                }
            }
        }

        void AddValidCommonMovesForPawn(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            AddValidCapturesForPawn(square, y, x, position, validMoves);
            AddForwardMoveForPawnIfValid(square, y, x, position, validMoves);
        }

        void AddEnPassantIfValid(Square square, int i, int j, ChessPosition position, IList<Move> validMoves)
        {
            if (i == (square.Color == Color.White ? 4 : 3))
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
        }

        void AddLongPawnMoveIfValid(Square square, int y, int x, ChessPosition position, IList<Move> validMoves)
        {
            if (y == (square.Color == Color.White ? 1 : 6))
            {
                var direction = RuleInfo.PawnMoveDirection[square.Color.Value];
                var yModifier = direction == Direction.North ? 2 : -2;
                if (position.GetNearestOccupiedSquare(x, y, direction, 2).Square.Piece == null)
                {
                    AddMoveToListIfValid(validMoves,
                        new Move(Piece.Pawn, x, x, y, y + yModifier, square.Color.Value, null), position);

                }
            }
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
            var repeatedPositionHashSet = new HashSet<ChessPosition>();
            foreach (var position in History)
            {
                if (!positionHashSet.Add(position))
                {
                    if (!repeatedPositionHashSet.Add(position))
                    {
                        return true;
                    }
                }
            }
            return false;
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
