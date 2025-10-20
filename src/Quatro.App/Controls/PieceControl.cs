using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quatro.Controls
{

    public class PieceControl : Control
    {
        static PieceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieceControl), new FrameworkPropertyMetadata(typeof(PieceControl)));
        }

        public Brush TopLeftBrush
        {
            get => (Brush)GetValue(TopLeftBrushProperty);
            set => SetValue(TopLeftBrushProperty, value);
        }

        public static readonly DependencyProperty TopLeftBrushProperty =
            DependencyProperty.Register(nameof(TopLeftBrush), typeof(Brush), typeof(PieceControl), new PropertyMetadata(Brushes.Transparent));

        public Brush TopRightBrush
        {
            get => (Brush)GetValue(TopRightBrushProperty);
            set => SetValue(TopRightBrushProperty, value);
        }

        public static readonly DependencyProperty TopRightBrushProperty =
            DependencyProperty.Register(nameof(TopRightBrush), typeof(Brush), typeof(PieceControl), new PropertyMetadata(Brushes.Transparent));

        public Brush BottomLeftBrush
        {
            get => (Brush)GetValue(BottomLeftBrushProperty);
            set => SetValue(BottomLeftBrushProperty, value);
        }

        public static readonly DependencyProperty BottomLeftBrushProperty =
            DependencyProperty.Register(nameof(BottomLeftBrush), typeof(Brush), typeof(PieceControl), new PropertyMetadata(Brushes.Transparent));

        public Brush BottomRightBrush
        {
            get => (Brush)GetValue(BottomRightBrushProperty);
            set => SetValue(BottomRightBrushProperty, value);
        }

        public static readonly DependencyProperty BottomRightBrushProperty =
            DependencyProperty.Register(nameof(BottomRightBrush), typeof(Brush), typeof(PieceControl), new PropertyMetadata(Brushes.Transparent));

        public bool IsDimmed
        {
            get => (bool)GetValue(IsDimmedProperty);
            set => SetValue(IsDimmedProperty, value);
        }

        public static readonly DependencyProperty IsDimmedProperty =
            DependencyProperty.Register(nameof(IsDimmed), typeof(bool), typeof(PieceControl), new PropertyMetadata(false));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(PieceControl), new PropertyMetadata(false));

        public bool IsWinning
        {
            get => (bool)GetValue(IsWinningProperty);
            set => SetValue(IsWinningProperty, value);
        }

        public static readonly DependencyProperty IsWinningProperty =
            DependencyProperty.Register(nameof(IsWinning), typeof(bool), typeof(PieceControl), new PropertyMetadata(false));

        public bool IsOnBoard
        {
            get => (bool)GetValue(IsOnBoardProperty);
            set => SetValue(IsOnBoardProperty, value);
        }

        public static readonly DependencyProperty IsOnBoardProperty =
            DependencyProperty.Register(nameof(IsOnBoard), typeof(bool), typeof(PieceControl), new PropertyMetadata(false));
    }
}
