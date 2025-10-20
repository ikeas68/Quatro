using System;
using System.Collections.Generic;
using System.Linq;

namespace Quatro48
{
    public sealed class BoardState
    {
        private readonly Piece[,] _grid = new Piece[4, 4];

        public Piece this[int row, int column]
        {
            get => _grid[row, column];
            private set => _grid[row, column] = value;
        }

        public bool TryPlacePiece(int row, int column, Piece piece)
        {
            if (_grid[row, column] != null)
            {
                return false;
            }

            _grid[row, column] = piece;
            return true;
        }

        public void RemovePiece(int row, int column) => _grid[row, column] = null;

        public IEnumerable<BoardPosition> GetEmptyCells()
        {
            for (var r = 0; r < 4; r++)
            {
                for (var c = 0; c < 4; c++)
                {
                    if (_grid[r, c] == null)
                    {
                        yield return new BoardPosition(r, c);
                    }
                }
            }
        }

        public IReadOnlyList<Piece> GetRow(int row) => Enumerable.Range(0, 4).Select(c => _grid[row, c]).ToArray();

        public IReadOnlyList<Piece> GetColumn(int column) => Enumerable.Range(0, 4).Select(r => _grid[r, column]).ToArray();

        public IReadOnlyList<Piece> GetDescendingDiagonal() => Enumerable.Range(0, 4).Select(i => _grid[i, i]).ToArray();

        public IReadOnlyList<Piece> GetAscendingDiagonal() => Enumerable.Range(0, 4).Select(i => _grid[3 - i, i]).ToArray();

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

        public bool CheckWin(out List<BoardPosition> winningCells, out PieceAttribute? winningAttribute)
        {
            winningCells = new List<BoardPosition>();
            winningAttribute = null;

            bool EvaluateLine(IEnumerable<BoardPosition> cells,
                              out List<BoardPosition> lineCells,
                              out PieceAttribute? attr)
            {
                lineCells = null;
                attr = null;

                var list = cells.ToList();
                var pieces = list.Select(cell => _grid[cell.Row, cell.Column]).ToList();
                if (pieces.Any(p => p == null))
                    return false;

                foreach (PieceAttribute attribute in Enum.GetValues(typeof(PieceAttribute)))
                {
                    var firstPiece = pieces[0];
                    var first = firstPiece.HasAttribute(attribute);

                    if (pieces.All(p => p != null && p.HasAttribute(attribute) == first))
                    {
                        lineCells = list;
                        attr = attribute;
                        return true;
                    }
                }
                return false;
            }

            // Lignes
            for (var r = 0; r < 4; r++)
                if (EvaluateLine(Enumerable.Range(0, 4).Select(c => new BoardPosition(r, c)),
                                 out winningCells, out winningAttribute))
                    return true;

            // Colonnes
            for (var c = 0; c < 4; c++)
                if (EvaluateLine(Enumerable.Range(0, 4).Select(r => new BoardPosition(r, c)),
                                 out winningCells, out winningAttribute))
                    return true;

            // Diagonales
            if (EvaluateLine(Enumerable.Range(0, 4).Select(i => new BoardPosition(i, i)),
                             out winningCells, out winningAttribute))
                return true;

            if (EvaluateLine(Enumerable.Range(0, 4).Select(i => new BoardPosition(3 - i, i)),
                             out winningCells, out winningAttribute))
                return true;

            // Pas de victoire
            winningCells = new List<BoardPosition>();
            winningAttribute = null;
            return false;
        }



    }
}
