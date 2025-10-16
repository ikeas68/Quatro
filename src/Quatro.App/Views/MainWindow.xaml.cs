using System.Windows;
using Quatro.ViewModels;

namespace Quatro.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new GameViewModel();
    }
}
