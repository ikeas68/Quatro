using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Quatro.Models;
using Quatro.Services;

namespace Quatro.ViewModels
{
    public sealed class GameViewModel : ViewModelBase
    {
        private readonly ComputerPlayer _computerPlayer = new ComputerPlayer();
        private readonly ObservableCollection<PieceViewModel> _pieces;
        private readonly ObservableCollection<BoardCellViewModel> _boardCells;
        private readonly ReadOnlyObservableCollection<PieceViewModel> _readOnlyPieces;
        private readonly ReadOnlyObservableCollection<BoardCellViewModel> _readOnlyBoardCells;
        private readonly ICollectionView _availablePieces;
        private readonly RelayCommand _placePieceCommand;
        private readonly RelayCommand _selectPieceCommand;
        private readonly RelayCommand _newGameCommand;

        private BoardState _board = new BoardState();
        private PieceViewModel _currentPiece;
        private GamePhase _phase;
        private string _statusMessage = string.Empty;
        private string _instructionMessage = string.Empty;
        private string _winnerMessage = string.Empty;
        private bool _isGameOver;
        private Brush _winningBrush;
        private int _selectedDifficulty = 1;

        public GameViewModel()
        {
            _pieces = new ObservableCollection<PieceViewModel>(PieceSet.All.Select(piece => new PieceViewModel(piece)));
            _boardCells = new ObservableCollection<BoardCellViewModel>();
            for (var row = 0; row < 4; row++)
            {
                for (var column = 0; column < 4; column++)
                {
                    _boardCells.Add(new BoardCellViewModel(row, column));
                }
            }

            _readOnlyPieces = new ReadOnlyObservableCollection<PieceViewModel>(_pieces);
            _readOnlyBoardCells = new ReadOnlyObservableCollection<BoardCellViewModel>(_boardCells);
            _availablePieces = CollectionViewSource.GetDefaultView(_pieces);
            _availablePieces.Filter = item => item is PieceViewModel vm && !vm.IsOnBoard;

            foreach (var piece in _pieces)
            {
                piece.PropertyChanged += OnPiecePropertyChanged;
            }

            _placePieceCommand = new RelayCommand(param =>
            {
                if (param is BoardCellViewModel cell)
                {
                    PlacePiece(cell);
                }
            },
            param => param is BoardCellViewModel cell && CanPlacePiece(cell));

            _selectPieceCommand = new RelayCommand(param =>
            {
                if (param is PieceViewModel piece)
                {
                    SelectPieceForComputer(piece);
                }
            },
            param => param is PieceViewModel piece && CanSelectPiece(piece));

            _newGameCommand = new RelayCommand(_ => StartNewGame());

            DifficultyLevels = new ReadOnlyCollection<int>(
                new[] { 1, 2, 3, 4 }
                );

            StartNewGame();
        }

        public ReadOnlyObservableCollection<PieceViewModel> Pieces => _readOnlyPieces;

        public ReadOnlyObservableCollection<BoardCellViewModel> BoardCells => _readOnlyBoardCells;

        public ICollectionView AvailablePieces => _availablePieces;

        public ReadOnlyCollection<int> DifficultyLevels { get; }

        public RelayCommand PlacePieceCommand => _placePieceCommand;

        public RelayCommand SelectPieceCommand => _selectPieceCommand;

        public RelayCommand NewGameCommand => _newGameCommand;

        public int SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (SetProperty(ref _selectedDifficulty, value))
                {
                    StartNewGame();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public string InstructionMessage
        {
            get => _instructionMessage;
            private set => SetProperty(ref _instructionMessage, value);
        }

        public string WinnerMessage
        {
            get => _winnerMessage;
            private set => SetProperty(ref _winnerMessage, value);
        }

        public bool IsBoardInteractionEnabled => _phase == GamePhase.HumanPlacing;

        public bool IsPieceSelectionEnabled => _phase == GamePhase.HumanSelecting;

        public bool IsGameOver
        {
            get => _isGameOver;
            private set
            {
                if (SetProperty(ref _isGameOver, value))
                {
                    _placePieceCommand.RaiseCanExecuteChanged();
                    _selectPieceCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Brush WinningBrush
        {
            get => _winningBrush;
            private set => SetProperty(ref _winningBrush, value);
        }

        public int RemainingPiecesCount => _pieces.Count(piece => piece.IsAvailable);

        private bool CanPlacePiece(BoardCellViewModel cell)
        {
            return !IsGameOver && _phase == GamePhase.HumanPlacing && cell.Piece == null && _currentPiece != null;
        }

        private bool CanSelectPiece(PieceViewModel piece)
        {
            return !IsGameOver && _phase == GamePhase.HumanSelecting && piece.IsAvailable;
        }

        private void StartNewGame()
        {
            _board = new BoardState();
            foreach (var cell in _boardCells)
            {
                cell.Piece = null;
                cell.IsWinning = false;
            }

            foreach (var piece in _pieces)
            {
                piece.IsAvailable = true;
                piece.IsSelected = false;
                piece.IsDimmed = false;
                piece.IsOnBoard = false;
                piece.IsWinning = false;
            }

            _currentPiece = null;
            _phase = GamePhase.None;
            StatusMessage = $"Difficulté : {_selectedDifficulty}";
            InstructionMessage = "L'ordinateur choisit une pièce pour vous.";
            WinnerMessage = string.Empty;
            WinningBrush = null;
            IsGameOver = false;
            RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
            RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
            RaisePropertyChanged(nameof(RemainingPiecesCount));
            _availablePieces.Refresh();
            GivePieceToHuman();
        }

        private void OnPiecePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PieceViewModel.IsOnBoard))
            {
                _availablePieces.Refresh();
            }
        }

        private void GivePieceToHuman()
        {
            var available = _pieces.Where(p => p.IsAvailable).Select(p => p.Piece).ToList();
            if (available.Count == 0)
            {
                HandleDraw();
                return;
            }

            var pieceModel = _computerPlayer.ChoosePieceForHuman(_board, available, _selectedDifficulty);
            var pieceViewModel = _pieces.First(p => ReferenceEquals(p.Piece, pieceModel));
            PrepareCurrentPiece(pieceViewModel, true);
            StatusMessage = $"Pièces restantes : {RemainingPiecesCount}";
            InstructionMessage = "Placez la pièce selectionné sur le plateau.";
            _phase = GamePhase.HumanPlacing;
            _placePieceCommand.RaiseCanExecuteChanged();
            _selectPieceCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
            RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
            RaisePropertyChanged(nameof(RemainingPiecesCount));
        }

        private void PrepareCurrentPiece(PieceViewModel piece, bool forHuman)
        {
            _currentPiece = piece;
            piece.IsAvailable = false;
            piece.IsSelected = true;
            piece.IsDimmed = forHuman;
            piece.IsOnBoard = false;
        }

        private void PlacePiece(BoardCellViewModel cell)
        {
            if (_currentPiece == null)
            {
                return;
            }

            if (!_board.TryPlacePiece(cell.Row, cell.Column, _currentPiece.Piece))
            {
                return;
            }

            cell.Piece = _currentPiece;
            cell.IsWinning = false;
            _currentPiece.IsOnBoard = true;
            _currentPiece.IsSelected = false;
            _currentPiece.IsDimmed = false;
            _currentPiece.IsWinning = false;
            _currentPiece = null;

            if (CheckForWinner(PlayerKind.Human))
            {
                return;
            }

            if (_board.IsBoardFull())
            {
                HandleDraw();
                return;
            }

            _phase = GamePhase.HumanSelecting;
            InstructionMessage = "Choisissez une pièce à donner à l'ordinateur.";
            StatusMessage = $"Pièces restantes : {RemainingPiecesCount}";
            _placePieceCommand.RaiseCanExecuteChanged();
            _selectPieceCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
            RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
            RaisePropertyChanged(nameof(RemainingPiecesCount));
        }

        private void SelectPieceForComputer(PieceViewModel piece)
        {
            piece.IsAvailable = false;
            piece.IsSelected = true;
            piece.IsDimmed = false;
            RaisePropertyChanged(nameof(RemainingPiecesCount));
            _phase = GamePhase.ComputerPlacing;
            RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
            RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
            StatusMessage = "L'ordinateur joue...";
            _placePieceCommand.RaiseCanExecuteChanged();
            _selectPieceCommand.RaiseCanExecuteChanged();

            ComputerPlacePiece(piece);

            if (IsGameOver)
            {
                return;
            }

            if (_board.IsBoardFull())
            {
                HandleDraw();
                return;
            }

            GivePieceToHuman();
        }

        private void ComputerPlacePiece(PieceViewModel piece)
        {
            var remainingPieces = _pieces.Where(p => p.IsAvailable).Select(p => p.Piece).ToList();
            var move = _computerPlayer.ChoosePlacement(_board, piece.Piece, remainingPieces, _selectedDifficulty);
            if (!_board.TryPlacePiece(move.Row, move.Column, piece.Piece))
            {
                var placed = false;
                foreach (var cell in _board.GetEmptyCells())
                {
                    if (_board.TryPlacePiece(cell.Row, cell.Column, piece.Piece))
                    {
                        move = new BoardPosition(cell.Row, cell.Column);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    return;
                }
            }

            var boardCell = _boardCells.First(c => c.Row == move.Row && c.Column == move.Column);
            boardCell.Piece = piece;
            boardCell.IsWinning = false;
            piece.IsOnBoard = true;
            piece.IsSelected = false;
            piece.IsDimmed = false;

            CheckForWinner(PlayerKind.Computer);
        }

        private bool CheckForWinner(PlayerKind player)
        {
            if (_board.CheckWin(out var winningCells, out var winningAttribute))
            {
                HighlightWin(winningCells, winningAttribute);
                WinnerMessage = player == PlayerKind.Human ? "Victoire de l'humain !" : "Victoire de l'ordinateur !";
                InstructionMessage = "Cliquez sur \"Nouvelle partie\" pour recommencer.";
                StatusMessage = string.Empty;
                _phase = GamePhase.GameOver;
                IsGameOver = true;
                RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
                RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
                _placePieceCommand.RaiseCanExecuteChanged();
                _selectPieceCommand.RaiseCanExecuteChanged();
                return true;
            }

            return false;
        }

        private void HighlightWin(IEnumerable<BoardPosition> winningCells, PieceAttribute? attribute)
        {
            foreach (var cell in _boardCells)
            {
                cell.IsWinning = false;
            }

            foreach (var piece in _pieces)
            {
                piece.IsWinning = false;
            }

            foreach (var position in winningCells)
            {
                var cell = _boardCells.First(c => c.Row == position.Row && c.Column == position.Column);
                cell.IsWinning = true;
                if (cell.Piece != null)
                {
                    cell.Piece.IsWinning = true;
                }
            }

            if (attribute.HasValue)
            {
                var winningCell = _boardCells.FirstOrDefault(c => c.IsWinning);
                if (winningCell != null && winningCell.Piece != null)
                {
                    var color = winningCell.Piece.Piece.GetAttributeColor(attribute.Value);
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();
                    WinningBrush = brush;
                    return;
                }
            }

            var fallback = new SolidColorBrush(PiecePalette.Magenta);
            fallback.Freeze();
            WinningBrush = fallback;
        }

        private void HandleDraw()
        {
            WinnerMessage = "Match nul";
            InstructionMessage = "Cliquez sur \"Nouvelle partie\" pour rejouer.";
            StatusMessage = string.Empty;
            _phase = GamePhase.GameOver;
            IsGameOver = true;
            RaisePropertyChanged(nameof(IsBoardInteractionEnabled));
            RaisePropertyChanged(nameof(IsPieceSelectionEnabled));
            _placePieceCommand.RaiseCanExecuteChanged();
            _selectPieceCommand.RaiseCanExecuteChanged();
        }
    }

    internal enum GamePhase
    {
        None,
        HumanPlacing,
        HumanSelecting,
        ComputerPlacing,
        GameOver
    }

    internal enum PlayerKind
    {
        Human,
        Computer
    }

    public class LevelConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int level = (int)value;
            switch (level)
            {
                case 1: return "1 - Débutant";
                case 2: return "2 - Apprenti";
                case 3: return "3 - Avancé";
                case 4: return "4 - Expert";
                default: return "?????";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();


        public override object ProvideValue(IServiceProvider serviceProvider) => this;

    }
}
