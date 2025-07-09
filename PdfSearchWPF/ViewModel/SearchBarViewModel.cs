using Microsoft.Win32;
using PdfSearchWPF.Commands;
using PdfSearchWPF.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace PdfSearchWPF.ViewModel
{
  internal class SearchBarViewModel : ViewModelBase
  {
    private readonly SearchEngine.SearchEngine _searchEngine;

    private string? _searchPath;
    private string? _searchTerm;
    private SearchOption _searchOptions;
    private bool _isSearching;

    public SearchBarViewModel(SearchEngine.SearchEngine searchEngine)
    {
      _searchEngine = searchEngine;


      SelectedFileTypes.CollectionChanged += SelectedFileTypes_CollectionChanged;
    }

    public ObservableCollection<string> AvailableFileTypes { get; } = ["All Files", ".pdf", ".docx", ".txt", ".xlsx", ".csv", ".xml", ".json"];

    public ObservableCollection<string> SelectedFileTypes { get; } = [];

    public bool CanSearch
    {
      get
      {
        if (_isSearching || string.IsNullOrEmpty(_searchPath) || string.IsNullOrEmpty(_searchTerm))
          return false;
        else
          return true;
      }
    }

    public string? SearchPath
    {
      get => _searchPath;
      set
      {
        SetField(ref _searchPath, value);
        OnPropertyChanged(nameof(CanSearch));
      }
    }

    public string? SearchTerm
    {
      get => _searchTerm;
      set
      {
        SetField(ref _searchTerm, value);
        OnPropertyChanged(nameof(CanSearch));
      }
    }

    public bool IsSearching
    {
      get => _isSearching;
      set
      {
        SetField(ref _isSearching, value);
        OnPropertyChanged(nameof(CanSearch));
      }
    }

    public SearchOption SearchOptions
    {
      get => _searchOptions;
      set
      {
        if (_searchOptions != value)
        {
          _searchOptions = value;
          OnPropertyChanged(nameof(SearchOptions));
          OnPropertyChanged(nameof(UseRegex));
          OnPropertyChanged(nameof(MatchWholeWord));
          OnPropertyChanged(nameof(MatchCase));
          OnPropertyChanged(nameof(ScanRecursive));
        }
      }
    }

    public bool UseRegex
    {
      get => SearchOptions.HasFlag(SearchOption.Regex);
      set
      {
        SetOption(SearchOption.Regex, value);
        OnPropertyChanged(nameof(UseRegex));
      }
    }

    public bool MatchWholeWord
    {
      get => SearchOptions.HasFlag(SearchOption.MatchWholeWord);
      set
      {
        SetOption(SearchOption.MatchWholeWord, value);
        OnPropertyChanged(nameof(MatchWholeWord));
      }
    }

    public bool MatchCase
    {
      get => SearchOptions.HasFlag(SearchOption.MatchCase);
      set
      {
        SetOption(SearchOption.MatchCase, value);
        OnPropertyChanged(nameof(MatchCase));
      }
    }

    public bool ScanRecursive
    {
      get => SearchOptions.HasFlag(SearchOption.Recursive);
      set
      {
        SetOption(SearchOption.Recursive, value);
        OnPropertyChanged(nameof(ScanRecursive));
      }
    }

    public ICommand ToggleRecursiveCommand => new RelayCommand(() => ScanRecursive = !ScanRecursive);
    public ICommand ToggleRegexCommand => new RelayCommand(() => UseRegex = !UseRegex);
    public ICommand ToggleMatchWholeWordCommand => new RelayCommand(() => MatchWholeWord = !MatchWholeWord);
    public ICommand ToggleMatchCaseCommand => new RelayCommand(() => MatchCase = !MatchCase);
    public ICommand ClearSearchStringCommand => new RelayCommand(() => SearchTerm = string.Empty);
    public ICommand ChooseFolderCommand => new RelayCommand(ExecuteChooseFolder, () => !IsSearching);
    public ICommand SearchCommand => new AsyncRelayCommand(ExecuteSearch, () => !IsSearching);

    private void SetOption(SearchOption option, bool isEnabled)
    {
      if (isEnabled)
        SearchOptions |= option;
      else
        SearchOptions &= ~option;

      OnPropertyChanged(nameof(SearchOptions));
    }

    private void ExecuteChooseFolder()
    {
      var dialog = new OpenFolderDialog();
      if (dialog.ShowDialog() == true)
      {
        SearchPath = dialog.FolderName;
      }
    }

    private async Task ExecuteSearch()
    {
      IsSearching = true;

      if (SelectedFileTypes.Count == 0)
        SelectedFileTypes.Add("All Files");

      try
      {
        _ = SearchPath ?? throw new ArgumentNullException("SearchPath");
        _ = SearchTerm ?? throw new ArgumentNullException("SearchString");

        await _searchEngine.SearchAsync(
          SearchPath,
          SearchTerm,
          SearchOptions,
          SelectedFileTypes.Contains("All Files") ? ["*.*"] : [.. SelectedFileTypes]
          );
      }
      finally
      {
        IsSearching = false;
      }
    }

    private void SelectedFileTypes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        if (!SelectedFileTypes.Contains("All Files"))
          return;

        Application.Current.Dispatcher.BeginInvoke(() =>
        {
          if (e.NewItems[0].ToString() == "All Files")
          {
            for (int i = SelectedFileTypes.Count - 1; i >= 0; i--)
            {
              if (SelectedFileTypes[i] != "All Files")
                SelectedFileTypes.RemoveAt(i);
            }
          }
          else
          {
            SelectedFileTypes.Remove("All Files");
          }
        });

      }
    }


  }
}
