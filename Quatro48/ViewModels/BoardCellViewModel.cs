namespace Quatro48
{
    public sealed class BoardCellViewModel : ViewModelBase
    {
        private PieceViewModel _piece;
        private bool _isWinning;

        public BoardCellViewModel(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public PieceViewModel Piece
        {
            get => _piece;
            set => SetProperty(ref _piece, value);
        }

        public bool IsWinning
        {
            get => _isWinning;
            set => SetProperty(ref _isWinning, value);
        }
    }
}
