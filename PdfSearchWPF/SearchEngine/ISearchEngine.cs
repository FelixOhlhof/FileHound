using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine.SearchStrategies;

namespace PdfSearchWPF.SearchEngine
{
  public interface ISearchEngine : IConfigurable
  {
    static string? Name { get; }
    static string? Description { get; }

    List<ISearchStrategy> Strategies { get; set; }

    event Action<SearchResult>? OnFileSearched;
    event Action? OnSearchFinished;
    event Action<int>? OnStartSearch;

    Task<IEnumerable<SearchResult>> SearchAsync(string searchPath, string searchTerm, SearchOption searchOption, List<string> fileExtensions, CancellationToken cancellationToken = default);

    static string GetName(ISearchEngine searchEngine)
    {
      var strategyType = searchEngine.GetType();
      var strategyNameProp = strategyType.GetProperty("Name", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      string strategyName = strategyNameProp?.GetValue(null)?.ToString() ?? strategyType.Name;
      return strategyName;
    }
  }
}