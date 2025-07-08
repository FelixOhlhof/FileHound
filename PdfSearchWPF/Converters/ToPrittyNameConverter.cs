using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  public class ToPrittyNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is not string)
        return value;

      string snakeToSpace = Regex.Replace((string)value, @"[_\-]", " ");

      string camelToSpace = Regex.Replace(snakeToSpace, @"(?<=[a-z])([A-Z])", " $1");

      TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
      string result = textInfo.ToTitleCase(camelToSpace.ToLower());
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
