namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  public class SearchStrategyOption
  {
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Type ValueType { get; set; } = typeof(string);
  }
}
