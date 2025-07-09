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
  }
}