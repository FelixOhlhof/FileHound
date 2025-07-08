namespace PdfSearchWPF.ViewModel
{
  class MainWindowViewModel : ViewModelBase
  {
    public SettingsViewModel SettingsVM { get; set; }
    public MainViewModel MainVW { get; set; }

    public Action? ViewModelChangedAction { get; set; }

    private ViewModelBase? _currentViewModel;

    public MainWindowViewModel(SettingsViewModel settingsVM, MainViewModel mainVW)
    {
      SettingsVM = settingsVM;
      MainVW = mainVW;
      CurrentViewModel = mainVW;

      mainVW.OpenSettingsAction = () => CurrentViewModel = SettingsVM;
      settingsVM.CloseAction = () => CurrentViewModel = mainVW;
    }

    public ViewModelBase? CurrentViewModel
    {
      get => _currentViewModel;
      set
      {
        SetField(ref _currentViewModel, value);
        ViewModelChangedAction?.Invoke();
      }
    }
  }
}
