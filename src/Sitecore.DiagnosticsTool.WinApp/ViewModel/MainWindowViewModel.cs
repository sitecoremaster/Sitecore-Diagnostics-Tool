namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Input;

  using Sitecore.DiagnosticsTool.WinApp.Command;
  using Sitecore.DiagnosticsTool.WinApp.Model;

  /// <summary>
  ///   The main ViewModel class for the wizard.
  ///   This class contains the various pages shown in the workflow and provides navigation  between the pages.
  /// </summary>
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    #region Fields

    private RelayCommand cancelCommand;

    private WizardPageViewModelBase currentPage;

    private RelayCommand moveNextCommand;

    private RelayCommand movePreviousCommand;

    private ReadOnlyCollection<WizardPageViewModelBase> pages;

    #endregion // Fields

    #region Constructors

    public MainWindowViewModel()
    {
      Source = new DataSource();
      CurrentPage = Pages[0];
    }

    #endregion // Constructor

    #region Commands

    #region CancelCommand

    public ICommand CancelCommand => cancelCommand ?? (cancelCommand = new RelayCommand(CancelOrder));

    private void CancelOrder(object o)
    {
      Source = null;
      OnRequestClose();
    }

    #endregion // CancelCommand

    #region MovePreviousCommand

    public ICommand MovePreviousCommand
    {
      get
      {
        return movePreviousCommand ?? (movePreviousCommand = new RelayCommand(
          MoveToPreviousPage,
          () => CanMoveToPreviousPage));
      }
    }

    private bool CanMoveToPreviousPage => 0 < CurrentPageIndex;

    public bool IsNextButtonEnabled { get; set; }

    private void MoveToPreviousPage(object o)
    {
      if (CanMoveToPreviousPage)
      {
        if (IsOnLastPage)
        {
          CurrentPage = Pages[0];
          RefreshSource();
        }
        else
        {
          CurrentPage = Pages[CurrentPageIndex - 1];
        }
      }
    }

    private void RefreshSource()
    {
      Source.Packages.Clear();
      Source.ReportPath = null;
      Source.ErrorMessage = null;
    }

    #endregion // MovePreviousCommand

    #region MoveNextCommand

    public ICommand MoveNextCommand
    {
      get
      {
        return moveNextCommand ?? (moveNextCommand = new RelayCommand(
          MoveToNextPage,
          () => CanMoveToNextPage));
      }
    }

    public bool CanMoveToNextPage
    {
      get
      {
        IsNextButtonEnabled = CurrentPage != null && CurrentPage.IsValid();
        OnPropertyChanged("IsNextButtonEnabled");
        return IsNextButtonEnabled;
      }
    }

    private void MoveToNextPage(object o)
    {
      if (!CanMoveToNextPage)
      {
        return;
      }

      if (CurrentPageIndex < Pages.Count - 1)
      {
        if (CurrentPage.GetType().GetInterfaces().Contains(typeof(ISyncableModel)))
        {
          ((ISyncableModel)CurrentPage).Sync();
        }
        CurrentPage = Pages[CurrentPageIndex + 1];

        if (CurrentPage.GetType().GetInterfaces().Contains(typeof(IExecutableModel)))
        {
          ((IExecutableModel)CurrentPage).Execute();
        }
      }
      else
      {
        OnRequestClose();
      }
    }

    #endregion // MoveNextCommand

    #endregion // Commands

    #region Properties

    public DataSource Source { get; private set; }

    public WizardPageViewModelBase CurrentPage
    {
      get
      {
        return currentPage;
      }
      private set
      {
        if (value == currentPage)
        {
          return;
        }

        if (currentPage != null)
        {
          currentPage.IsCurrentPage = false;
        }

        currentPage = value;

        if (currentPage != null)
        {
          currentPage.IsCurrentPage = true;
        }

        OnPropertyChanged("CurrentPage");
        OnPropertyChanged("IsOnLastPage");
        OnPropertyChanged("IsOnFirstPage");
      }
    }

    public bool IsOnLastPage => CurrentPageIndex == Pages.Count - 1;

    public bool IsOnFirstPage => CurrentPageIndex == 0;

    public ReadOnlyCollection<WizardPageViewModelBase> Pages
    {
      get
      {
        if (pages == null)
        {
          CreatePages();
        }

        return pages;
      }
    }

    #endregion // Properties

    #region Events

    public event EventHandler RequestClose;

    #endregion // Events

    #region Private Helpers

    private void CreatePages()
    {
      var welcome = new WelcomePageViewModel(Source);
      var resources = new ResourcesPageViewModel(Source);

      //var configuration = new ConfigurationPageViewModel(source);
      var diagnostics = new DiagnosticsPageViewModel(Source);
      var results = new ResultsPageViewModel(Source);

      var pages = new List<WizardPageViewModelBase>
      {
        welcome,
        resources,

        //configuration,
        diagnostics,
        results
      };

      this.pages = new ReadOnlyCollection<WizardPageViewModelBase>(pages);
    }

    private int CurrentPageIndex
    {
      get
      {
        if (CurrentPage != null)
        {
          return Pages.IndexOf(CurrentPage);
        }

        return -1;
      }
    }

    private void OnRequestClose()
    {
      var handler = RequestClose;
      handler?.Invoke(this, EventArgs.Empty);
    }

    #endregion // Private Helpers

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion // INotifyPropertyChanged Members
  }
}