namespace PdfSearchWPF.SearchEngine
{
  public interface IConfigurable
  {
    Settings? Settings { get; set; }
    IEnumerable<SettingDefinition> SupportedSettings { get; }
  }
}