﻿using System.Windows;

namespace PdfSearchWPF.Helpers
{
  public class BindingProxy : Freezable
  {
    protected override Freezable CreateInstanceCore() => new BindingProxy();

    public object Data
    {
      get => GetValue(DataProperty);
      set => SetValue(DataProperty, value);
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
  }
}
