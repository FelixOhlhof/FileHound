using PdfSearchWPF.Model;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  public interface ISearchStrategy : IConfigurable
  {
    string Name { get; }
    List<string> SupportedFileExtensions { get; }
    bool CanHandle(string filePath);
    SearchResult SearchFile(string filePath, string searchTerm, SearchOption options);
  }
}
