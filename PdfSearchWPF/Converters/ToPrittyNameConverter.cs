using PdfSearchWPF.Extensions;
using System.Globalization;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  public class ToPrittyNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is not string)
        return value;

      return ((string)value).ToPrittyString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
