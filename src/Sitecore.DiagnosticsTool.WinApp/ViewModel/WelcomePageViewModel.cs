namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;

  public class WelcomePageViewModel : WizardPageViewModelBase
  {
    #region Constructors

    public WelcomePageViewModel(DataSource source)
      : base(source)
    {
    }

    #endregion

    #region WizardPageViewModelBase Members

    public override string DisplayName => Strings.PageDisplayName_Welcome;

    public override string Icon => Strings.Icon_Welcome;

    public override string Title => Strings.PageTitle_Welcome;

    public override bool IsValid()
    {
      return true;
    }

    #endregion
  }
}