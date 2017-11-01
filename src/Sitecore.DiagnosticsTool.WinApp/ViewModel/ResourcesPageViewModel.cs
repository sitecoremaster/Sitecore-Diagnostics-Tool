namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Input;

  using Sitecore.DiagnosticsTool.WinApp.Command;
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;
  using Sitecore.DiagnosticsTool.WinApp.View;

  public class ResourcesPageViewModel : WizardPageViewModelBase, ISyncableModel
  {
    #region Constructors

    public ResourcesPageViewModel(DataSource source)
      : base(source)
    {
    }

    #endregion

    #region Fields

    private RelayCommand addResourceCommand;

    #endregion

    #region Commands

    public ICommand AddResourceCommand
    {
      get
      {
        return addResourceCommand ?? (addResourceCommand = new RelayCommand(
          () =>
          {
            var view = new ResourceDetailsPageView(Resources);
            Resources.Add(view);
            (view.DataContext as ResourceDetailsViewModel).LoadPackageCommand.Execute(null);
          },
          () => true));
      }
    }

    #endregion

    #region WizardPageViewModelBase Members

    public override string DisplayName => Strings.PageDisplayName_Resources;

    public override string Icon => Strings.Icon_Resources;

    public ObservableCollection<ResourceDetailsPageView> Resources { get; set; } = new ObservableCollection<ResourceDetailsPageView>();

    public override string Title => Strings.PageTitle_Resources;

    public override bool IsValid()
    {
      return Resources.Any() && Resources.All(resourceDetailsPageView =>
      {
        var resourceDetailsViewModel = resourceDetailsPageView.DataContext as ResourceDetailsViewModel;
        return resourceDetailsViewModel != null && resourceDetailsViewModel.IsResourceValid;
      });
    }

    #endregion

    #region ISyncableModel Members

    public void Sync()
    {
      foreach (var resource in Resources)
      {
        var package = resource.DataContext as ResourceDetailsViewModel;
        if (package == null)
        {
          continue;
        }

        var packageSource = new SourcePackageModel
        {
          Path = package.PackagePath,
          Roles = package.ServerRoles
        };
        Source.Packages.Add(packageSource);
      }

      Resources.Clear();
    }

    #endregion
  }
}