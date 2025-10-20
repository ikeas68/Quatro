using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quatro.ViewModels;

namespace Quatro.Controls
{

    public class BoardCellControl : Control
    {
        static BoardCellControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoardCellControl), new FrameworkPropertyMetadata(typeof(BoardCellControl)));
        }

        public PieceViewModel Piece
        {
            get => GetValue(PieceProperty) as PieceViewModel;
            set => SetValue(PieceProperty, value);
        }

        public static readonly DependencyProperty PieceProperty =
            DependencyProperty.Register(nameof(Piece), typeof(PieceViewModel), typeof(BoardCellControl), new PropertyMetadata(null));

        public bool IsWinning
        {
            get => (bool)GetValue(IsWinningProperty);
            set => SetValue(IsWinningProperty, value);
        }

        public static readonly DependencyProperty IsWinningProperty =
            DependencyProperty.Register(nameof(IsWinning), typeof(bool), typeof(BoardCellControl), new PropertyMetadata(false));

        public Brush WinningBrush
        {
            get => GetValue(WinningBrushProperty) as Brush;
            set => SetValue(WinningBrushProperty, value);
        }

        public static readonly DependencyProperty WinningBrushProperty =
            DependencyProperty.Register(nameof(WinningBrush), typeof(Brush), typeof(BoardCellControl), new PropertyMetadata(null));

        public ICommand Command
        {
            get => GetValue(CommandProperty) as ICommand;
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(BoardCellControl), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(BoardCellControl), new PropertyMetadata(null));

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!IsEnabled)
            {
                return;
            }

            var command = Command;
            if (command != null)
            {
                var parameter = CommandParameter ?? DataContext;
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
