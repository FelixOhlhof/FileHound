using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class BooleanToVisibilityConverter : IValueConverter
  {
    // Converts bool to Visibility (true => Visible, false => Collapsed)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool b && b)
        return Visibility.Visible;
      else
        return Visibility.Collapsed;
    }

    // Converts back Visibility to bool (Visible => true, others => false)
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility v && v == Visibility.Visible)
        return true;
      else
        return false;
    }
  }
}
