using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine;

namespace PdfSearchWPF.DesignTime
{
  public class DemoSettingsViewModel
  {
    public List<SettingSection> StrategySettings { get; set; }
    public SettingSection SearchEngineSettings { get; set; }


    public DemoSettingsViewModel()
    {
      StrategySettings = new List<SettingSection>
      {
        new() {
          Name = "DemoStrategy",
          DisplayName = "Demo Strategy Settings",
          Entries = new List<SettingEntry>
          {
            {
              new SettingEntry(new SettingDefinition("CountAll", "Counts all matches", true, SettingType.Bool), true)
            },
            {
              new SettingEntry(new SettingDefinition("BufferSize", "Size of buffer", 1024, SettingType.Number), 1024)
            },
            {
              new SettingEntry(new SettingDefinition("Encoding", "Encoding", new List<string>{"abc", "def"}, SettingType.Enum), "out")
            }
          }
        },
        new() {
          Name = "DemoStrategy",
          DisplayName = "Demo Strategy Settings",
          Entries = new List<SettingEntry>
          {
            {
              new SettingEntry(new SettingDefinition("CountAll", "Counts all matches", true, SettingType.Bool), true)
            },
            {
              new SettingEntry(new SettingDefinition("BufferSize", "Size of buffer", 1024, SettingType.Date), DateTime.Now)
            }
          }
        }
      };

      SearchEngineSettings = new()
      {
        Name = "DemoSearchEngineSettings",
        DisplayName = "Demo Search Engine Settings",
        Entries = new List<SettingEntry>
          {
            {
              new SettingEntry(new SettingDefinition("CountAll", "Counts all matches", true, SettingType.Bool), true)
            },
            {
              new SettingEntry(new SettingDefinition("BufferSize", "Size of buffer", 1024, SettingType.Number), 1024)
            }
          }
      };
    }
  }
}
