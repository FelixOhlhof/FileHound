using PdfSearchWPF.SearchEngine;
using System.Windows;

namespace PdfSearchWPF.ViewModel
{
  internal class MainViewModel : ViewModelBase
  {
    private double _windowHeight = 150; // initial window height
    private double _windowMaxHeight = 150;

    public double WindowHeight
    {
      get => _windowHeight;
      set => SetField(ref _windowHeight, value);
    }

    public double WindowMaxHeight
    {
      get => _windowMaxHeight;
      set => SetField(ref _windowMaxHeight, value);
    }

    public SearchBarViewModel SearchBarVM { get; set; }
    public SearchResultViewModel SearchResultVM { get; set; }

    public MainViewModel()
    {
      SearchEngine.SearchEngine searchEngine = new(
          options: new SearchEngineOptions
          {
            ExecuteMultipleStrategies = true,
            MaxCpuUsagePercent = 0.8,
          },
          strategies:
          [
            new SearchEngine.SearchStrategies.TextSearchStrategy(),
            new SearchEngine.SearchStrategies.PdfSearchStrategy(),
          ]
          );

      SearchBarVM = new SearchBarViewModel(searchEngine);
      SearchResultVM = new SearchResultViewModel();


      searchEngine.OnStartSearch += (totalFiles) =>
         Application.Current.Dispatcher.Invoke(() =>
         {
           setExpandedWindowSize();
           SearchResultVM.StartNew(totalFiles);
         });

      searchEngine.OnFileSearched += (result) =>
         Application.Current.Dispatcher.Invoke(() =>
         {
           SearchResultVM.AddResult(result);
         });

      searchEngine.OnSearchFinished += () =>
         Application.Current.Dispatcher.Invoke(() =>
         {
           SearchResultVM.Finished();
         });
    }

    private void setExpandedWindowSize()
    {
      WindowMaxHeight = double.PositiveInfinity;

      if (WindowHeight <= 400)
        WindowHeight = 400;
    }
  }
}
