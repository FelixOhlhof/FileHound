﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PdfSearchWPF.Converters
{
  public class BoolToBrushConverter : IValueConverter
  {
    public Brush TrueBrush { get; set; } = Brushes.LightBlue;
    public Brush FalseBrush { get; set; } = Brushes.Transparent;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool b && b)
        return TrueBrush;
      return FalseBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

}
