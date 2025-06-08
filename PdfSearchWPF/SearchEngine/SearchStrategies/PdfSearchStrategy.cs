
using PdfSearchWPF.Model;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using IO = System.IO;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  internal class PdfSearchStrategy(SearchStrategyOptions? strategyOptions = null) : ISearchStrategy
  {
    private readonly SearchStrategyOptions? _strategyOptions = strategyOptions;

    public string Name => "PDF Search";

    public List<string> SupportedFileExtensions => ["pdf"];

    public IEnumerable<SearchStrategyOption> GetSupportedOptions() =>
      [
        new SearchStrategyOption
        {
          Name = "CountAll",
          Description = "Counts all occurrences.",
          ValueType = typeof(bool)
        }
      ];

    public bool CanHandle(string filePath)
        => IO.Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

    public SearchResult SearchFile(string filePath, string searchTerm, SearchOption searchOptions)
    {
      bool useRegex = searchOptions.HasFlag(SearchOption.Regex);
      bool matchWholeWord = searchOptions.HasFlag(SearchOption.MatchWholeWord);
      bool matchCase = searchOptions.HasFlag(SearchOption.MatchCase);

      int occurrences = 0;
      Regex? regex = null;

      if (useRegex)
      {
        var pattern = matchWholeWord
            ? $@"\b{searchTerm}\b"
            : searchTerm;

        regex = new Regex(pattern,
            matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
      }

      var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

      try
      {
        using var document = PdfDocument.Open(filePath);

        foreach (var page in document.GetPages())
        {
          string text = page.Text;

          if (useRegex && regex != null)
          {
            occurrences += regex.Matches(text).Count;
          }
          else
          {
            int index = 0;
            while ((index = text.IndexOf(searchTerm, index, comparison)) != -1)
            {
              if (matchWholeWord)
              {
                bool isStartOk = index == 0 || !char.IsLetterOrDigit(text[index - 1]);
                bool isEndOk = index + searchTerm.Length >= text.Length ||
                               !char.IsLetterOrDigit(text[index + searchTerm.Length]);

                if (isStartOk && isEndOk)
                {
                  occurrences++;
                }
              }
              else
              {
                occurrences++;
              }

              index += searchTerm.Length;
            }
          }

          bool countAll = true;
          _strategyOptions?.TryGet("CountAll", out countAll);
          if (!countAll && occurrences > 0)
            break;
        }
      }
      catch (Exception ex)
      {
        return new SearchResult
        {
          FileName = filePath,
          SearchTerm = searchTerm,
          Occurrences = 0,
          Error = new Exceptions.SearchResultException(filePath, "error executing pdf search", ex)
        };
      }

      return new SearchResult
      {
        FileName = filePath,
        SearchTerm = searchTerm,
        Occurrences = occurrences
      };
    }
  }
}


