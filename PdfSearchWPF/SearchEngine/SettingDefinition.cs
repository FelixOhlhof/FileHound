namespace PdfSearchWPF.SearchEngine
{
  public enum SettingType
  {
    Text,
    Number,
    Bool,
    Date,
  }

  public class SettingDefinition(string name, string description, object standardValue, SettingType valueType)
  {
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public object StandardValue { get; set; } = standardValue;
    public SettingType ValueType { get; set; } = valueType;
  }
}
