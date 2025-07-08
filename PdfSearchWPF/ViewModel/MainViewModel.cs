using PdfSearchWPF.Commands;
using System.Windows;
using System.Windows.Input;

namespace PdfSearchWPF.ViewModel
{
  internal class MainViewModel : ViewModelBase
  {
    public SearchBarViewModel SearchBarVM { get; set; }
    public SearchResultViewModel SearchResultVM { get; set; }

    public Action? OpenSettingsAction { get; set; }

    public MainViewModel(SearchEngine.SearchEngine searchEngine)
    {
      SearchBarVM = new SearchBarViewModel(searchEngine);
      SearchResultVM = new SearchResultViewModel();

      searchEngine.OnStartSearch += (totalFiles) =>
         Application.Current.Dispatcher.Invoke(() =>
         {
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

    public ICommand OpenSettingsCommand => new RelayCommand(() => OpenSettingsAction?.Invoke());

  }
}
