using PdfSearchWPF.Model;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  public interface ISearchStrategy
  {
    string Name { get; }
    List<string> SupportedFileExtensions { get; }
    IEnumerable<SearchStrategyOption> GetSupportedOptions();
    bool CanHandle(string filePath);
    SearchResult SearchFile(string filePath, string searchTerm, SearchOption options);
  }

}
