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

    static string GetName(ISearchStrategy searchStrategy)
    {
      var strategyType = searchStrategy.GetType();
      var strategyNameProp = strategyType.GetProperty("Name", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      string strategyName = strategyNameProp?.GetValue(null)?.ToString() ?? strategyType.Name;
      return strategyName;
    }
  }
}
