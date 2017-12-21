namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System.ComponentModel;

  using Sitecore.DiagnosticsTool.WinApp.Model;

  /// <summary>
  ///   Abstract base class for all pages shown in the wizard.
  /// </summary>
  public abstract class WizardPageViewModelBase : INotifyPropertyChanged
  {
    #region Fields

    private bool isCurrentPage;

    #endregion

    #region Constructors

    protected WizardPageViewModelBase(DataSource source)
    {
      Source = source;
    }

    #endregion

    #region Properties

    public DataSource Source { get; set; }

    public abstract string DisplayName { get; }

    public abstract string Icon { get; }

    public abstract string Title { get; }

    public bool IsCurrentPage
    {
      get
      {
        return isCurrentPage;
      }
      set
      {
        if (value == isCurrentPage)
        {
          return;
        }

        isCurrentPage = value;
        OnPropertyChanged("IsCurrentPage");
      }
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Returns true if the user has filled in this page properly
    ///   and the wizard should allow the user to progress to the next page in the workflow.
    /// </summary>
    public abstract bool IsValid();

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}