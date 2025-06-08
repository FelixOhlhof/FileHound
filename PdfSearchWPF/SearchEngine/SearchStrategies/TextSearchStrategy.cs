using PdfSearchWPF.Model;
using System.Text;
using System.Text.RegularExpressions;
using IO = System.IO;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  internal class TextSearchStrategy(SearchStrategyOptions? strategyOptions = null) : ISearchStrategy
  {
    private readonly SearchStrategyOptions? _strategyOptions = strategyOptions;

    public string Name => "Text Search";

    public List<string> SupportedFileExtensions => ["*"];

    public IEnumerable<SearchStrategyOption> GetSupportedOptions() =>
        [
            new SearchStrategyOption
            {
                Name = "BufferSize",
                Description = "Size of read buffer in bytes.",
                ValueType = typeof(int)
            },
            new SearchStrategyOption
            {
                Name = "Encoding",
                Description = "Choose between UTF8, Unicode or ASCII.",
                ValueType = typeof(string)
            },
            new SearchStrategyOption
            {
                Name = "CountAll",
                Description = "Counts all occurences.",
                ValueType = typeof(bool)
            }
        ];

    public bool CanHandle(string filePath)
        => IO.Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase);

    public SearchResult SearchFile(
    string filePath,
    string searchTerm,
    SearchOption searchOptions)
    {
      bool useRegex = searchOptions.HasFlag(SearchOption.Regex);
      bool matchWholeWord = searchOptions.HasFlag(SearchOption.MatchWholeWord);
      bool matchCase = searchOptions.HasFlag(SearchOption.MatchCase);

      int overlapSize = searchTerm.Length > 1 ? searchTerm.Length - 1 : 0;

      string previousChunkTail = string.Empty;
      int occurrences = 0;

      var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
      Regex? regex = null;

      if (useRegex)
      {
        var pattern = matchWholeWord
            ? $@"\b{searchTerm}\b"
            : searchTerm;

        regex = new Regex(pattern,
            matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
      }

      string encoding = "UTF8"; // default
      _strategyOptions?.TryGet("Encoding", out encoding);

      using var reader = new IO.StreamReader(filePath, encoding: stringToEncoding(encoding));

      int bufferSize = 16 * 1024; // default
      _strategyOptions?.TryGet("BufferSize", out bufferSize);

      char[] buffer = new char[bufferSize];
      int read;

      while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
      {
        string chunk = previousChunkTail + new string(buffer, 0, read);

        if (useRegex && regex != null)
        {
          occurrences += regex.Matches(chunk).Count;
        }
        else
        {
          int index = 0;
          while ((index = chunk.IndexOf(searchTerm, index, comparison)) != -1)
          {
            // Check whole word boundaries if needed
            if (matchWholeWord)
            {
              bool isStartOk = index == 0 || !char.IsLetterOrDigit(chunk[index - 1]);
              bool isEndOk = index + searchTerm.Length >= chunk.Length ||
                             !char.IsLetterOrDigit(chunk[index + searchTerm.Length]);

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

        if (!countAll)
          break;

        // Save the last N characters to carry over to next chunk
        previousChunkTail = chunk.Substring(Math.Max(0, chunk.Length - overlapSize));
      }

      return new SearchResult
      {
        FileName = filePath,
        SearchTerm = searchTerm,
        Occurrences = occurrences
      };
    }

    private Encoding stringToEncoding(string encoding)
    {
      switch (encoding.ToLower())
      {
        case "ascii":
          return Encoding.ASCII;
        case "unicode":
          return Encoding.Unicode;
        default: return Encoding.UTF8;
      }
    }
  }
}
