using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PdfSearchWPF.Converters
{
  public class ItemSelectedToBrushConverter : IMultiValueConverter
  {
    public Brush SelectedBrush { get; set; } = Brushes.LightBlue;
    public Brush DefaultBrush { get; set; } = Brushes.Transparent;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length < 2)
        return DefaultBrush;

      var item = values[0];
      var selectedItems = values[1] as ObservableCollection<string>;

      if (selectedItems != null && item != null && selectedItems.Contains(item.ToString()))
        return SelectedBrush;

      return DefaultBrush;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
