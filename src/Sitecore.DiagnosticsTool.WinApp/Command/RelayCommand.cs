namespace Sitecore.DiagnosticsTool.WinApp.Command
{
  #region Usings

  using System;
  using System.Diagnostics;
  using System.Windows.Input;

  #endregion

  /// <summary>
  ///   A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
  ///   The default return value for the CanExecute method is 'true'.
  /// </summary>
  internal class RelayCommand<T> : ICommand
  {
    #region Constructors

    public RelayCommand(Action<T> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    ///   Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException(nameof(execute));
      }

      _execute = execute;
      _canExecute = canExecute;
    }

    #endregion // Constructors

    #region Fields

    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;

    #endregion // Fields

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute?.Invoke((T)parameter) ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested += value;
        }
      }
      remove
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
        }
      }
    }

    public void Execute(object parameter)
    {
      _execute((T)parameter);
    }

    #endregion // ICommand Members
  }

  /// <summary>
  ///   A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
  ///   The default return value for the CanExecute method is 'true'.
  /// </summary>
  internal class RelayCommand : ICommand
  {
    #region Fields

    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    #endregion // Fields

    #region Constructors

    /// <summary>
    ///   Creates a new command that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public RelayCommand(Action execute)
      : this(execute, null)
    {
    }

    /// <summary>
    ///   Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException(nameof(execute));
      }

      _execute = execute;
      _canExecute = canExecute;
    }

    #endregion // Constructors

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute?.Invoke() ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested += value;
        }
      }
      remove
      {
        if (_canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
        }
      }
    }

    public void Execute(object parameter)
    {
      _execute();
    }

    #endregion // ICommand Members
  }
}