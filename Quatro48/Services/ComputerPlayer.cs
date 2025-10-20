using System;
using System.Collections.Generic;
using System.Linq;

namespace Quatro48
{

    public sealed class ComputerPlayer
    {
        private readonly Random _random = new Random();

        public BoardPosition ChoosePlacement(BoardState board, Piece piece, IReadOnlyCollection<Piece> remainingPieces, int difficulty)
        {
            var winningMove = FindWinningMove(board, piece);
            if (winningMove is BoardPosition best)
            {
                return best;
            }

            if (difficulty >= 3)
            {
                var safeMoves = GetSafeMoves(board, piece, remainingPieces).ToList();
                if (safeMoves.Count > 0)
                {
                    return ChoosePreferredMove(safeMoves, difficulty);
                }
            }

            var moves = board.GetEmptyCells().Select(cell => new BoardPosition(cell.Row, cell.Column)).ToList();
            return moves.Count == 0 ? default : ChoosePreferredMove(moves, difficulty);
        }

        public Piece ChoosePieceForHuman(BoardState board, IReadOnlyCollection<Piece> remainingPieces, int difficulty)
        {
            return ChoosePiece(board, remainingPieces, difficulty, preferHarder: true);
        }

        public Piece ChoosePieceForComputer(BoardState board, IReadOnlyCollection<Piece> remainingPieces, int difficulty)
        {
            return ChoosePiece(board, remainingPieces, difficulty, preferHarder: false);
        }

        private Piece ChoosePiece(BoardState board, IReadOnlyCollection<Piece> remainingPieces, int difficulty, bool preferHarder)
        {
            if (!remainingPieces.Any())
            {
                throw new InvalidOperationException("No remaining pieces available.");
            }

            var pieces = remainingPieces.ToList();

            if (difficulty >= 2)
            {
                var safePieces = pieces.Where(piece => !CreatesImmediateWinForOpponent(board, piece)).ToList();
                if (safePieces.Count > 0)
                {
                    pieces = safePieces;
                }
            }

            if (difficulty >= 4)
            {
                return (preferHarder ? pieces
                    .OrderByDescending(piece => CountPotentialWins(board, piece))
                    : pieces.OrderBy(piece => CountPotentialWins(board, piece)))
                    .ThenBy(_ => _random.Next())
                    .First();
            }

            if (difficulty >= 3)
            {
                return (preferHarder ? pieces.OrderByDescending(CountDistinctAttributes) : pieces.OrderBy(CountDistinctAttributes))
                    .ThenBy(_ => _random.Next())
                    .First();
            }

            return pieces[_random.Next(pieces.Count)];
        }

        private BoardPosition ChoosePreferredMove(IReadOnlyList<BoardPosition> moves, int difficulty)
        {
            if (difficulty <= 2)
            {
                return moves[_random.Next(moves.Count)];
            }

            return moves
                .OrderBy(move => HeuristicScore(move))
                .ThenBy(_ => _random.Next())
                .First();
        }

        private static int HeuristicScore(BoardPosition position)
        {
            var centerRow = 1.5;
            var centerColumn = 1.5;
            var distance = Math.Abs(position.Row - centerRow) + Math.Abs(position.Column - centerColumn);
            return (int)(distance * 100);
        }

        private IEnumerable<BoardPosition> GetSafeMoves(BoardState board, Piece piece, IReadOnlyCollection<Piece> remainingPieces)
        {
            foreach (var cell in board.GetEmptyCells())
            {
                board.TryPlacePiece(cell.Row, cell.Column, piece);
                var createsForcedWin = remainingPieces.All(other => CreatesImmediateWinForOpponent(board, other));
                board.RemovePiece(cell.Row, cell.Column);
                if (!createsForcedWin)
                {
                    yield return new BoardPosition(cell.Row, cell.Column);
                }
            }
        }

        private static BoardPosition? FindWinningMove(BoardState board, Piece piece)
        {
            foreach (var cell in board.GetEmptyCells())
            {
                board.TryPlacePiece(cell.Row, cell.Column, piece);
                var isWin = board.CheckWin(out _, out _);
                board.RemovePiece(cell.Row, cell.Column);
                if (isWin)
                {
                    return new BoardPosition(cell.Row, cell.Column);
                }
            }

            return null;
        }

        private bool CreatesImmediateWinForOpponent(BoardState board, Piece piece)
        {
            foreach (var cell in board.GetEmptyCells())
            {
                board.TryPlacePiece(cell.Row, cell.Column, piece);
                var isWin = board.CheckWin(out _, out _);
                board.RemovePiece(cell.Row, cell.Column);
                if (isWin)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountDistinctAttributes(Piece piece)
        {
            var score = 0;
            if (piece.IsLight) score++;
            if (piece.IsWarm) score++;
            if (piece.IsVivid) score++;
            if (piece.IsEarthy) score++;
            return score;
        }

        private int CountPotentialWins(BoardState board, Piece piece)
        {
            var count = 0;
            foreach (var cell in board.GetEmptyCells())
            {
                board.TryPlacePiece(cell.Row, cell.Column, piece);
                if (board.CheckWin(out _, out _))
                {
                    count++;
                }

                board.RemovePiece(cell.Row, cell.Column);
            }

            return count;
        }
    }
}
