﻿using System.Globalization;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  public class InverseBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is not bool b || !b;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
  }
}
