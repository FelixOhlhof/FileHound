using PdfSearchWPF.Exceptions;
using PdfSearchWPF.Model;
using System.Collections.ObjectModel;

namespace PdfSearchWPF.ViewModel
{
  internal class SearchResultViewModel : ViewModelBase
  {
    private ObservableCollection<SearchResult> _searchResults = new();

    private SearchProgress _searchProgress = new();

    public SearchProgress SearchProgress
    {
      get { return _searchProgress; }
      private set { SetField(ref _searchProgress, value); }
    }

    public ObservableCollection<SearchResult> SearchResults
    {
      get => _searchResults;
      set
      {
        if (SetField(ref _searchResults, value))
        {
          OnPropertyChanged(nameof(HasResults));
        }
      }
    }

    public bool HasResults => SearchResults != null && SearchResults.Count > 0;

    public void AddResult(SearchResult result)
    {
      if (result.Error?.GetType() == typeof(FileTypeNotSupportedException))
        return;

      SearchProgress.Current += 1;
      OnPropertyChanged(nameof(SearchProgress));

      SearchResults.Add(result);
      OnPropertyChanged(nameof(HasResults));
    }

    public void StartNew(int totalFiles)
    {
      SearchProgress = new SearchProgress { Total = totalFiles, IsFinished = false };
      SearchResults = [];
    }

    public void Finished()
    {
      SearchProgress.IsFinished = true;
      OnPropertyChanged(nameof(SearchProgress));
    }
  }
}
