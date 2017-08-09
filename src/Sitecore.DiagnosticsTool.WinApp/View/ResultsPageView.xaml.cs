namespace Sitecore.DiagnosticsTool.WinApp.View
{
  using System.Diagnostics;
  using System.Windows.Navigation;

  /// <summary>
  ///   Interaction logic for ResultsPageView.xaml
  /// </summary>
  public partial class ResultsPageView
  {
    #region Constructors

    public ResultsPageView()
    {
      InitializeComponent();
    }

    #endregion

    #region Handlers

    private void LinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo("explorer.exe", $"\"{e.Uri.AbsoluteUri}\""));
      e.Handled = true;
    }

    #endregion
  }
}