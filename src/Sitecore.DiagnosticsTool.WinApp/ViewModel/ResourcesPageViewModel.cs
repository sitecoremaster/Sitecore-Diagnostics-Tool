using Sitecore.Diagnostics.FileSystem.Extensions;

namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System.Collections.ObjectModel;
  using System.Linq;

  using Microsoft.Win32;

  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.WinApp.Command;
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;
  using Sitecore.DiagnosticsTool.WinApp.View;

  using ICommand = System.Windows.Input.ICommand;

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
          _ =>
          {
            var view = new ResourceDetailsPageView(Resources);
            Resources.Add(view);

            var openFileDialog = new OpenFileDialog
            {
              Filter = "SSPG package|*.zip"
            };

            if (openFileDialog.ShowDialog() != true)
            {
              return;
            }

            var file = new FileSystem().ParseFile(openFileDialog.FileName);
            var dir = new FileSystem().CreateUniqueTempFolder();

            file.ExtractZipToDirectory(dir);
            if (PackageHelper.IsLegacyPackage(dir))
            {
              var viewModel = (ResourceDetailsViewModel)view.DataContext;
              viewModel.LoadPackageCommand.Execute(file.FullName);
            }
            else
            {
              var directories = PackageHelper.ExtractMegaPackage(file);

              if (true)
              {
                var viewModel = (ResourceDetailsViewModel)view.DataContext;
                viewModel.LoadPackageCommand.Execute(directories.First().FullName);
              }

              foreach (var subdir in directories.Skip(1))
              {
                var nextView = new ResourceDetailsPageView(Resources);
                Resources.Add(nextView);

                var viewModel = (ResourceDetailsViewModel)nextView.DataContext;
                viewModel.LoadPackageCommand.Execute(subdir.FullName);
              }
            }
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