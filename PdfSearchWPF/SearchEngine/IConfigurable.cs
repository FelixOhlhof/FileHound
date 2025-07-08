namespace PdfSearchWPF.SearchEngine
{
  public interface IConfigurable
  {
    string Name { get; }
    string Description { get; }
    Settings? Settings { get; set; }
    IEnumerable<SettingDefinition> SupportedSettings { get; }
  }
}