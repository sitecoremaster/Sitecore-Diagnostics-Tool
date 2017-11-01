namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;

  public class ResultsPageViewModel : WizardPageViewModelBase
  {
    #region Constructors

    public ResultsPageViewModel(DataSource source)
      : base(source)
    {
    }

    #endregion

    #region WizardPageViewModelBase Members

    public override string DisplayName => Strings.PageDisplayName_Results;

    public override string Title => Strings.PageTitle_Results;

    public override bool IsValid() => true;

    public override string Icon => Strings.Icon_Results;

    #region Properties

    public string ReportPath => Source.ReportPath;

    public string ErrorMessage => Source.ErrorMessage;

    #endregion Properties

    #endregion
  }
}