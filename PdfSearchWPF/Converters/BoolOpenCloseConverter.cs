using System.Globalization;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  public class BoolOpenCloseConverter : IValueConverter
  {
    private readonly string _open = ">>";
    private readonly string _close = "<<";


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool b && b)
        return _close;
      return _open;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
