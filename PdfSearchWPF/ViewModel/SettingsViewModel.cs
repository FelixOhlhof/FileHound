﻿
using PdfSearchWPF.Commands;
using PdfSearchWPF.Extensions;
using PdfSearchWPF.Model;
using PdfSearchWPF.SearchEngine;
using PdfSearchWPF.SearchEngine.SearchStrategies;
using System.Windows.Input;

namespace PdfSearchWPF.ViewModel
{
  internal class SettingsViewModel : ViewModelBase
  {
    private readonly SearchEngine.SearchEngine _searchEngine;
    private SettingSection _searchEngineSettings = new();
    private List<SettingSection> _strategySettings = new();

    public Action? CloseAction { get; set; }

    public SettingsViewModel(SearchEngine.SearchEngine searchEngine)
    {
      _searchEngine = searchEngine;

      LoadSettings();
    }

    public ICommand CloseCommand => new RelayCommand(() => CloseAction?.Invoke());

    public ICommand SaveCommand => new RelayCommand(() =>
    {
      SaveSettings();
      CloseAction?.Invoke();
    });

    public List<SettingSection> StrategySettings
    {
      get => _strategySettings;
      set => SetField(ref _strategySettings, value);
    }

    public SettingSection SearchEngineSettings
    {
      get => _searchEngineSettings;
      set => SetField(ref _searchEngineSettings, value);
    }

    private void LoadSettings()
    {
      _searchEngineSettings = new SettingSection
      {
        Name = ISearchEngine.GetName(_searchEngine),
        DisplayName = $"{ISearchEngine.GetName(_searchEngine).ToPrittyString()} Settings"
      };

      foreach (var def in _searchEngine.SupportedSettings)
      {
        object? value = null;
        _searchEngine.Settings?.TryGet(def.Name, out value);
        _searchEngineSettings.Entries.Add(new SettingEntry(def, value ?? def.StandardValue));
      }

      _strategySettings = [];

      foreach (ISearchStrategy strategy in _searchEngine.Strategies)
      {
        var section = new SettingSection
        {
          Name = ISearchStrategy.GetName(strategy),
          DisplayName = $"{ISearchStrategy.GetName(strategy).ToPrittyString()} Settings"
        };

        section.Entries.Add(new SettingEntry(new SearchEngine.SettingDefinition("IsActive", "De-/Activates this Search Strategy", strategy.IsActivated, SearchEngine.SettingType.Bool), strategy.IsActivated));

        foreach (var def in strategy.SupportedSettings)
        {
          object? value = null;
          strategy.Settings?.TryGet(def.Name, out value);
          section.Entries.Add(new SettingEntry(def, value ?? def.StandardValue));
        }

        _strategySettings.Add(section);
      }
    }

    private void SaveSettings()
    {
      _searchEngine.Settings ??= new();

      foreach (var entry in _searchEngineSettings.Entries)
      {
        _searchEngine.Settings.Set(entry.Definition.Name, entry.Value);
      }

      _searchEngine.Settings.Save($"{ISearchEngine.GetName(_searchEngine)}.json");

      foreach (var section in _strategySettings)
      {
        var strategy = _searchEngine.Strategies.First(x => ISearchStrategy.GetName(x) == section.Name);
        strategy.Settings ??= new();

        strategy.IsActivated = (bool)(section.Entries.Find(x => x.Definition.Name == "IsActive")?.Value ?? true);

        foreach (var entry in section.Entries)
        {
          strategy.Settings.Set(entry.Definition.Name, entry.Value);
        }

        strategy.Settings.Save($"{ISearchStrategy.GetName(strategy)}.json");

      }
    }
  }

}
