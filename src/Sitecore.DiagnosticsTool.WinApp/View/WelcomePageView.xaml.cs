namespace Sitecore.DiagnosticsTool.WinApp.View
{
  using System.Diagnostics;
  using System.Windows.Navigation;

  /// <summary>
  ///   Interaction logic for WelcomePageView.xaml
  /// </summary>
  public partial class WelcomePageView
  {
    #region Constructors

    public WelcomePageView()
    {
      InitializeComponent();
    }

    #endregion

    #region Handlers

    private void LinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    #endregion
  }
}