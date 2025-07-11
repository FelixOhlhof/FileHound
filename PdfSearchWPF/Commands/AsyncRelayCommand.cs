﻿using System.Windows.Input;

namespace PdfSearchWPF.Commands
{
  public class AsyncRelayCommand : ICommand
  {
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
      return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
      if (!CanExecute(parameter))
        return;

      _isExecuting = true;
      RaiseCanExecuteChanged();

      try
      {
        await _execute();
      }
      finally
      {
        _isExecuting = false;
        RaiseCanExecuteChanged();
      }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
