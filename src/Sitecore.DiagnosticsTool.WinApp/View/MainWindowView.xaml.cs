namespace Sitecore.DiagnosticsTool.WinApp.View
{
  using System.Diagnostics;
  using System.Windows.Navigation;

  /// <summary>
  ///   Interaction logic for MainWindowView.xaml
  /// </summary>
  public partial class MainWindowView
  {
    #region Constructors

    public MainWindowView()
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