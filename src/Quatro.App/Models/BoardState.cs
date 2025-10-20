using System;
using System.Collections.Generic;
using System.Linq;

namespace Quatro.Models
{

    public sealed class BoardState
    {
        private readonly Piece?[,] _grid = new Piece?[4, 4];

        public Piece? this[int row, int column]
        {
            get => _grid[row, column];
            private set => _grid[row, column] = value;
        }

        public bool TryPlacePiece(int row, int column, Piece piece)
        {
            if (_grid[row, column] is not null)
            {
                return false;
            }

            _grid[row, column] = piece;
            return true;
        }

        public void RemovePiece(int row, int column) => _grid[row, column] = null;

        public IEnumerable<(int row, int column)> GetEmptyCells()
        {
            for (var r = 0; r < 4; r++)
            {
                for (var c = 0; c < 4; c++)
                {
                    if (_grid[r, c] is null)
                    {
                        yield return (r, c);
                    }
                }
            }
        }

        public IReadOnlyList<Piece?> GetRow(int row) => Enumerable.Range(0, 4).Select(c => _grid[row, c]).ToArray();

        public IReadOnlyList<Piece?> GetColumn(int column) => Enumerable.Range(0, 4).Select(r => _grid[r, column]).ToArray();

        public IReadOnlyList<Piece?> GetDescendingDiagonal() => Enumerable.Range(0, 4).Select(i => _grid[i, i]).ToArray();

        public IReadOnlyList<Piece?> GetAscendingDiagonal() => Enumerable.Range(0, 4).Select(i => _grid[3 - i, i]).ToArray();

        public bool CheckWin(out List<(int row, int column)> winningCells, out PieceAttribute? winningAttribute)
        {
            winningCells = new List<(int, int)>();
            winningAttribute = null;

            bool EvaluateLine(IEnumerable<(int r, int c)> cells)
            {
                var list = cells.ToList();
                var pieces = list.Select(cell => _grid[cell.r, cell.c]).ToList();
                if (pieces.Any(p => p is null))
                {
                    return false;
                }

                foreach (PieceAttribute attribute in Enum.GetValues(typeof(PieceAttribute)))
                {
                    var first = pieces[0]!.HasAttribute(attribute);
                    if (pieces.All(p => p!.HasAttribute(attribute) == first))
                    {
                        winningCells = list;
                        winningAttribute = attribute;
                        return true;
                    }
                }

                return false;
            }

            for (var r = 0; r < 4; r++)
            {
                if (EvaluateLine(Enumerable.Range(0, 4).Select(c => (r, c))))
                {
                    return true;
                }
            }

            for (var c = 0; c < 4; c++)
            {
                if (EvaluateLine(Enumerable.Range(0, 4).Select(r => (r, c))))
                {
                    return true;
                }
            }

            if (EvaluateLine(Enumerable.Range(0, 4).Select(i => (i, i))))
            {
                return true;
            }

            if (EvaluateLine(Enumerable.Range(0, 4).Select(i => (3 - i, i))))
            {
                return true;
            }

            return false;
        }

        public bool IsBoardFull() => !GetEmptyCells().Any();

        public BoardState Clone()
        {
            var copy = new BoardState();
            for (var r = 0; r < 4; r++)
            {
                for (var c = 0; c < 4; c++)
                {
                    copy._grid[r, c] = _grid[r, c];
                }
            }

            return copy;
        }
    }
}
