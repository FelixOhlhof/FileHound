using PdfSearchWPF.Exceptions;
using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine.SearchStrategies;
using System.ComponentModel;
using IO = System.IO;

namespace PdfSearchWPF.SearchEngine
{
  public class SearchEngine(SearchEngineOptions options, List<ISearchStrategy> strategies) : INotifyPropertyChanged
  {
    private int _maxDegreeOfParallelism = calcMaxDegreeOfParallelism(options.MaxCpuUsagePercent);
    private SearchEngineOptions _options = options;
    private List<ISearchStrategy> _strategies = strategies;

    public List<ISearchStrategy> Strategies
    {
      get { return _strategies; }
      set
      {
        _strategies = value;
        OnPropertyChanged(nameof(Strategies));
      }
    }

    public SearchEngineOptions Options
    {
      get { return _options; }
      set
      {
        _maxDegreeOfParallelism = calcMaxDegreeOfParallelism(_options.MaxCpuUsagePercent);
        _options = value;
        OnPropertyChanged(nameof(Options));
      }
    }

    public event Action<int>? OnStartSearch;
    public event Action? OnSearchFinished;
    public event Action<SearchResult>? OnFileSearched;


    public async Task<IEnumerable<SearchResult>> SearchAsync(string searchPath, string searchTerm, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
      var results = new List<SearchResult>();

      var semaphore = new SemaphoreSlim(_maxDegreeOfParallelism);

      var tasks = new List<Task>();

      var files = IO.Directory.GetFiles(searchPath, "*.*", searchOption.HasFlag(SearchOption.Recursive) ? IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly).ToList();

      OnStartSearch?.Invoke(files.Count);

      foreach (string file in files)
      {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var task = Task.Run(() =>
        {
          try
          {
            foreach (var strategy in Strategies)
            {
              SearchResult result;

              if (!strategy.CanHandle(file))
              {
                result = new SearchResult { FileName = file, SearchTerm = searchTerm, Strategy = strategy.Name, Error = new FileTypeNotSupportedException(file) };
                results.Add(result);
                OnFileSearched?.Invoke(result);
                continue;
              }

              try
              {
                result = strategy.SearchFile(file, searchTerm, searchOption) ?? throw new NullReferenceException($"search strategy {strategy.Name} returned null");
              }
              catch (Exception e)
              {
                result = new SearchResult { FileName = file, SearchTerm = searchTerm, Error = new SearchResultException(file, $"error while executing {strategy.Name} on file {file}", e) };
              }

              result.Strategy = strategy.Name;

              results.Add(result);
              OnFileSearched?.Invoke(result);

              if (!Options.ExecuteMultipleStrategies)
                break;
            }
          }
          finally
          {
            semaphore.Release();
          }
        }, cancellationToken);

        tasks.Add(task);
      }

      await Task.WhenAll(tasks);

      OnSearchFinished?.Invoke();

      return results;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static int calcMaxDegreeOfParallelism(double maxCpuUsagePercent) => Math.Max(1, (int)(Environment.ProcessorCount * maxCpuUsagePercent));
  }
}
