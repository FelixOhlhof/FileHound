using PdfSearchWPF.Exceptions;
using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine.SearchStrategies;
using IO = System.IO;

namespace PdfSearchWPF.SearchEngine
{
  public class SearchEngine : ISearchEngine
  {
    private int _maxDegreeOfParallelism;
    private double _maxCpuUsagePercent = 0.8;
    private bool _executeMultipleStrategies = false;
    private List<ISearchStrategy> _strategies;
    private Settings? _settings;

    public SearchEngine(List<ISearchStrategy> strategies, Settings? settings = null)
    {
      setSettings(settings);

      _maxDegreeOfParallelism = calcMaxDegreeOfParallelism(_maxCpuUsagePercent);
      _strategies = strategies;
      _settings = settings;
    }

    public static string Name => "Search Engine";

    public static string Description => "Default Search Engine";

    public IEnumerable<SettingDefinition> SupportedSettings => [
        new SettingDefinition
        (
          name: "CountAll",
          description: "Counts all occurrences.",
          standardValue: true,
          valueType: SettingType.Bool
        )
      ];

    public Settings? Settings
    {
      get { return _settings; }
      set
      {
        setSettings(value);

        _maxDegreeOfParallelism = calcMaxDegreeOfParallelism(_maxCpuUsagePercent);
        _settings = value;
      }
    }

    public List<ISearchStrategy> Strategies { get => _strategies; set => _strategies = value; }

    public event Action<int>? OnStartSearch;
    public event Action? OnSearchFinished;
    public event Action<SearchResult>? OnFileSearched;


    public async Task<IEnumerable<SearchResult>> SearchAsync(string searchPath, string searchTerm, SearchOption searchOption, List<string> fileExtensions, CancellationToken cancellationToken = default)
    {
      var results = new List<SearchResult>();

      var semaphore = new SemaphoreSlim(_maxDegreeOfParallelism);

      var tasks = new List<Task>();

      var files = fileExtensions
        .SelectMany(x => IO.Directory.GetFiles(
          searchPath,
          $"*.{x.Replace(".", "")}",
          searchOption.HasFlag(SearchOption.Recursive)
          ? IO.SearchOption.AllDirectories
          : IO.SearchOption.TopDirectoryOnly))
        .ToList();

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

              if (!strategy.IsActivated)
                continue;

              if (!strategy.CanHandle(file))
              {
                result = new SearchResult { FileName = file, SearchTerm = searchTerm, Strategy = strategy.GetType().Name, Error = new FileTypeNotSupportedException(file) };
                results.Add(result);
                OnFileSearched?.Invoke(result);
                continue;
              }

              try
              {
                result = strategy.SearchFile(file, searchTerm, searchOption) ?? throw new NullReferenceException($"search strategy {strategy.GetType().Name} returned null");
              }
              catch (Exception e)
              {
                result = new SearchResult { FileName = file, SearchTerm = searchTerm, Error = new SearchResultException(file, $"error while executing {strategy.GetType().Name} on file {file}", e) };
              }

              result.Strategy = strategy.GetType().Name;

              results.Add(result);
              OnFileSearched?.Invoke(result);

              if (!_executeMultipleStrategies)
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

    private void setSettings(Settings? settings)
    {
      settings?.TryGet("MaxCpuUsagePercent", out _maxCpuUsagePercent);
      settings?.TryGet("ExecuteMultipleStrategies", out _executeMultipleStrategies);
    }

    private static int calcMaxDegreeOfParallelism(double maxCpuUsagePercent) => Math.Max(1, (int)(Environment.ProcessorCount * maxCpuUsagePercent));

  }
}
