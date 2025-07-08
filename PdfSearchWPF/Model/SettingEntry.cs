using PdfSearchWPF.SearchEngine;
using System.ComponentModel;

namespace PdfSearchWPF.Model
{
  public class SettingEntry : INotifyPropertyChanged
  {
    public SettingDefinition Definition { get; }

    private object _value;
    public object Value
    {
      get => _value;
      set
      {
        if (!_value?.Equals(value) ?? value != null)
        {
          _value = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
      }
    }

    public SettingEntry(SettingDefinition definition, object value)
    {
      Definition = definition;
      _value = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
  }

}
