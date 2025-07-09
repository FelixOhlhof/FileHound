using PdfSearchWPF.Model;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  public interface ISearchStrategy : IConfigurable, IActivateable
  {
    static string? Name { get; }
    static string? Description { get; }

    static List<string>? SupportedFileExtensions { get; }
    bool CanHandle(string filePath);
    SearchResult SearchFile(string filePath, string searchTerm, SearchOption options);
  }
}
