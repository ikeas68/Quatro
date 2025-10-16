using System.Windows.Media;
using Quatro.Models;

namespace Quatro.ViewModels;

public sealed class PieceViewModel : ViewModelBase
{
    private bool _isAvailable = true;
    private bool _isSelected;
    private bool _isDimmed;
    private bool _isOnBoard;
    private bool _isWinning;

    public PieceViewModel(Piece piece)
    {
        Piece = piece;
        QuadrantBrushes = piece.QuadrantBrushes;
    }

    public Piece Piece { get; }

    private SolidColorBrush[] QuadrantBrushes { get; }

    public Brush TopLeftBrush => QuadrantBrushes[0];

    public Brush TopRightBrush => QuadrantBrushes[1];

    public Brush BottomLeftBrush => QuadrantBrushes[2];

    public Brush BottomRightBrush => QuadrantBrushes[3];

    public bool IsAvailable
    {
        get => _isAvailable;
        set => SetProperty(ref _isAvailable, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool IsDimmed
    {
        get => _isDimmed;
        set => SetProperty(ref _isDimmed, value);
    }

    public bool IsOnBoard
    {
        get => _isOnBoard;
        set => SetProperty(ref _isOnBoard, value);
    }

    public bool IsWinning
    {
        get => _isWinning;
        set => SetProperty(ref _isWinning, value);
    }
}
