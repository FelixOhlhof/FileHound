using PdfSearchWPF.SearchEngine;
using PdfSearchWPF.SearchEngine.SearchStrategies;
using PdfSearchWPF.ViewModel;
using System.Windows;

namespace PdfSearchWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private bool _canResize = false;

    public MainWindow()
    {
      InitializeComponent();

      SearchEngine.SearchEngine searchEngine = new(
        strategies:
        [
          new TextSearchStrategy(Settings.Load($"{TextSearchStrategy.Name}.json")),
          new PdfSearchStrategy(Settings.Load($"{PdfSearchStrategy.Name}.json")),
        ],
        Settings.Load($"{SearchEngine.SearchEngine.Name}.json"));

      MainWindowViewModel viewModel =
        new(
          new SettingsViewModel(searchEngine),
          new MainViewModel(searchEngine)
          );

      viewModel.ViewModelChangedAction += () =>
      {
        if (viewModel.CurrentViewModel?.GetType() != typeof(MainViewModel) && !_canResize)
        {
          MaxHeight = double.PositiveInfinity;
          Height = 400;
        }
        else if (!_canResize)
        {
          MaxHeight = 170;
        }
      };

      searchEngine.OnStartSearch += (_) =>
      {
        _canResize = true;
        MaxHeight = double.PositiveInfinity;
        Height = 400;
      };

      this.DataContext = viewModel;
    }
  }
}