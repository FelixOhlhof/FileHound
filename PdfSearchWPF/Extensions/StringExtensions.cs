using System.Globalization;
using System.Text.RegularExpressions;

namespace PdfSearchWPF.Extensions
{
  public static partial class StringExtensions
  {
    public static string ToPrittyString(this string str)
    {
      string snakeToSpace = SnakeCaseRegex().Replace(str, " ");

      string camelToSpace = CamelCaseRegex().Replace(snakeToSpace, " $1");

      TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
      string result = textInfo.ToTitleCase(camelToSpace.ToLower());
      return result;
    }

    [GeneratedRegex(@"[_\-]")]
    private static partial Regex SnakeCaseRegex();

    [GeneratedRegex(@"(?<=[a-z])([A-Z])")]
    private static partial Regex CamelCaseRegex();
  }
}
