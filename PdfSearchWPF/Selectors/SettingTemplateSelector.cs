using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine;
using System.Windows;
using System.Windows.Controls;

namespace PdfSearchWPF.Selectors
{
  public class SettingTemplateSelector : DataTemplateSelector
  {
    public DataTemplate? BoolTemplate { get; set; }
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? NumberTemplate { get; set; }
    public DataTemplate? DateTemplate { get; set; }
    public DataTemplate? EnumTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
      if (item is SettingEntry entry)
      {
        return entry.Definition.ValueType switch
        {
          SettingType.Bool => BoolTemplate,
          SettingType.Text => TextTemplate,
          SettingType.Number => NumberTemplate,
          SettingType.Date => DateTemplate,
          SettingType.Enum => EnumTemplate,
          _ => base.SelectTemplate(item, container)
        };
      }

      return base.SelectTemplate(item, container);
    }
  }

}
