namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System.Collections.ObjectModel;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using System.Windows;

  using Microsoft.Win32;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Core.Extensions;
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
            if (IsLegacyPackage(file))
            {
              var viewModel = (ResourceDetailsViewModel)view.DataContext;
              viewModel.LoadPackageCommand.Execute(file.FullName);
            }
            else
            {
              var extracted = file.ExtractZipToDirectory();
              var directories = extracted.GetDirectories();
              foreach (var directory in directories)
              {
                foreach (var link in directory.GetFiles("*.link", SearchOption.AllDirectories))
                {
                  var value = link.ReadAllText();
                  if (!value.StartsWith("[link] "))
                  {
                    continue;
                  }

                  var relativePath = value.Substring("[link] ".Length).Replace("%20", " ");
                  var fullPath = link.FileSystem.Internals.Path.GetFullPath(link.FileSystem.Internals.Path.Combine(link.Directory.FullName, relativePath));
                  var target = link.FileSystem.ParseFile(fullPath);
                  if (!target.Exists)
                  {                    
                    continue;
                  }

                  link.Delete();
                  link.Directory.Create();
                  target.CopyTo(link.Directory, link.NameWithoutExtension);
                }
              }

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

    private static bool IsLegacyPackage(IFile file)
    {
      // legacy package has PackageInfo.xml in zip root
      using (var zip = file.FileSystem.ZipManager.OpenRead(file))
      {
        var fileNamesInZipRoot = zip.Entries
          .Where(x => !x.FullName.Contains('/'))
          .Select(x => x.FullName)
          .Distinct()
          .ToArray();

        return fileNamesInZipRoot.Any(x => x == "PackageInfo.xml");
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