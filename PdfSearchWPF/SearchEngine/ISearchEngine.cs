using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine.SearchStrategies;

namespace PdfSearchWPF.SearchEngine
{
  public interface ISearchEngine : IConfigurable
  {
    List<ISearchStrategy> Strategies { get; set; }

    event Action<SearchResult>? OnFileSearched;
    event Action? OnSearchFinished;
    event Action<int>? OnStartSearch;

    Task<IEnumerable<SearchResult>> SearchAsync(string searchPath, string searchTerm, SearchOption searchOption, CancellationToken cancellationToken = default);
  }
}