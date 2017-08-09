namespace Sitecore.DiagnosticsTool.WinApp.View
{
  using System.Collections.ObjectModel;
  using Sitecore.DiagnosticsTool.WinApp.ViewModel;

  public partial class ResourceDetailsPageView
  {
    #region Fields

    public readonly ObservableCollection<ResourceDetailsPageView> SourcePackages;

    #endregion

    #region Constructors

    public ResourceDetailsPageView(ObservableCollection<ResourceDetailsPageView> collection)
    {
      InitializeComponent();
      DataContext = new ResourceDetailsViewModel
      {
        View = this
      };
      SourcePackages = collection;
    }

    #endregion
  }
}