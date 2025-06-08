using PdfSearchWPF.Exceptions;

namespace PdfSearchWPF.Model
{
  public class SearchResult
  {
    public required string FileName { get; set; }
    public int Occurrences { get; set; }
    public required string SearchTerm { get; set; }
    public SearchOption? SearchOptions { get; set; }
    public int DurationMs { get; set; }
    public string? Strategy { get; set; }
    public SearchResultException? Error { get; set; }
  }
}
