namespace PdfSearchWPF.Model
{
  public class SettingSection
  {
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public List<SettingEntry> Entries { get; set; } = new();
  }
}
