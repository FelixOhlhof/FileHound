﻿using System.Globalization;
using System.Windows.Data;

namespace PdfSearchWPF.Converters
{
  [ValueConversion(typeof(Exception), typeof(string))]
  public class ObjectToTypeName : IValueConverter
  {
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value?.GetType()?.Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
