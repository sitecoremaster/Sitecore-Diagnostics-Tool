namespace Sitecore.DiagnosticsTool.WinApp.View
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///   Interaction logic for ResourcesPageView.xaml
  /// </summary>
  public partial class ResourcesPageView
  {
    #region Constructors

    public ResourcesPageView()
    {
      InitializeComponent();
    }

    #endregion

    #region Handlers

    private void OnAddButtonClicked(object sender, RoutedEventArgs e)
    {
      ((Window.GetWindow(this)?.Content as MainWindowView)?.FindName("MainScrollViewer") as ScrollViewer)?.ScrollToBottom();
    }

    #endregion Handlers
  }
}