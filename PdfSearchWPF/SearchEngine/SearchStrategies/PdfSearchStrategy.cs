
using PdfSearchWPF.Model;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using IO = System.IO;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  internal class PdfSearchStrategy(Settings? settings = null) : ISearchStrategy
  {
    public string Name => "PDF Search";

    public string Description => "Default PDF Search Strategy";

    public List<string> SupportedFileExtensions => ["pdf"];

    public Settings? Settings { get; set; } = settings;

    public IEnumerable<SettingDefinition> SupportedSettings => [
        new SettingDefinition
        (
          name: "CountAll",
          description: "Counts all occurrences.",
          standardValue: true,
          valueType: SettingType.Bool
        )
      ];

    public bool CanHandle(string filePath)
        => IO.Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);


    public SearchResult SearchFile(string filePath, string searchTerm, SearchOption searchOptions)
    {
      bool useRegex = searchOptions.HasFlag(SearchOption.Regex);
      bool matchWholeWord = searchOptions.HasFlag(SearchOption.MatchWholeWord);
      bool matchCase = searchOptions.HasFlag(SearchOption.MatchCase);
      bool countAll = true;
      Settings?.TryGet("CountAll", out countAll);

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
          if (!countAll && occurrences > 0)
            break;

          string text = page.Text;

          if (useRegex && regex != null)
          {
            if (countAll)
              occurrences += regex.Matches(text).Count;
            else
            {
              occurrences = regex.IsMatch(text) ? 1 : 0;
              break;
            }
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

              if (!countAll && occurrences > 0)
                break;
            }
          }
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


