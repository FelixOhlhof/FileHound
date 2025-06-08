using PdfSearchWPF.ViewModel;
using System.Windows;

namespace PdfSearchWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      MainViewModel viewModel = new MainViewModel();
      this.DataContext = viewModel;
    }
  }
}