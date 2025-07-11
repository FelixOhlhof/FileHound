using PdfSearchWPF.Model;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using IO = System.IO;
using OCR = Tesseract;

namespace PdfSearchWPF.SearchEngine.SearchStrategies
{
  internal class PdfSearchStrategy(Settings? settings = null, bool isActivated = true) : ISearchStrategy
  {
    private readonly Lazy<OCR.TesseractEngine> _tesseractEngine = new(() =>
      {
        string language = "eng";
        settings?.TryGet("OCR Language", out language);
        return new OCR.TesseractEngine("C:\\Users\\felix\\Downloads\\", language);
      }
    );

    public static string Name => "PDF Search";

    public static string Description => "Default PDF Search Strategy";

    public static List<string> SupportedFileExtensions => ["pdf"];

    public Settings? Settings { get; set; } = settings;

    public bool IsActivated { get; set; } = isActivated;

    public IEnumerable<SettingDefinition> SupportedSettings => [
        new SettingDefinition
        (
          name: "CountAll",
          description: "Counts all occurrences.",
          standardValue: true,
          valueType: SettingType.Bool
        ),
        new SettingDefinition
        (
          name: "OCR",
          description: "Scan Images with OCR in PDF.",
          standardValue: false,
          valueType: SettingType.Bool
        ),
        new SettingDefinition
        (
          name: "OCR Language",
          description: "Language in which the PDF is written.",
          standardValue : new List<string>{"eng", "deu"},
          valueType: SettingType.Enum
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
      bool ocr = false;
      Settings?.TryGet("OCR", out ocr);

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
          text += ocr ? getOcrOfPage(page) : string.Empty;

          if (useRegex && regex != null)
          {
            if (countAll)
            {
              occurrences += regex.Matches(text).Count;
            }
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

    private string getOcrOfPage(Page page)
    {
      string extractedText = string.Empty;

      var imgs = page.GetImages();

      foreach (var pdfImage in imgs)
      {
        if (pdfImage.TryGetPng(out var pngBytes))
        {
          var pixImg = OCR.Pix.LoadFromMemory(pngBytes);
          using var ocrPage = _tesseractEngine.Value.Process(pixImg);
          extractedText += ocrPage.GetText();
        }
        else
        {
          var pixImg = OCR.Pix.LoadFromMemory(pdfImage.RawBytes.ToArray());
          using var ocrPage = _tesseractEngine.Value.Process(pixImg);
          extractedText += ocrPage.GetText();
        }
      }

      return extractedText;
    }
  }
}


