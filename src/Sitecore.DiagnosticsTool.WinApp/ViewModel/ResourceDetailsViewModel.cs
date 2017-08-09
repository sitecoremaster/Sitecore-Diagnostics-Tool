namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Media;
  using Microsoft.Win32;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.WinApp.Command;
  using Sitecore.DiagnosticsTool.WinApp.Resources;
  using Sitecore.DiagnosticsTool.WinApp.View;

  public class ResourceDetailsViewModel : INotifyPropertyChanged
  {
    #region Fields

    private string packagePath;
    private List<ServerRole> serverRoles = new List<ServerRole>();
    public ITestResourceContext Context { get; set; }

    #endregion

    #region Commands

    public ICommand CloseCommand
    {
      get
      {
        return new RelayCommand(() => { View.SourcePackages.Remove(View); }, () => true);
      }
    }

    public ICommand InfoCommand
    {
      get
      {
        return new RelayCommand(() =>
        {
          if (!IsPackageLoaded)
          {
            return;
          }
          CreatePackageDetailsPopup();
        });
      }
    }

    private void CreatePackageDetailsPopup()
    {
      #region Initialize componets

      var brushConverter = new BrushConverter();
      var popup = new Popup
      {
        StaysOpen = false
      };
      var borderBrush = (Brush)brushConverter.ConvertFrom("#cccccc");
      var border = new Border
      {
        BorderThickness = new Thickness(0.7),
        BorderBrush = borderBrush,
        Background = Brushes.White
      };

      var mainStackPanel = new StackPanel();

      #endregion

      #region Header

      var textBrush = (Brush)brushConverter.ConvertFrom("#595959");

      var header = new TextBlock
      {
        TextAlignment = TextAlignment.Center,
        Text = "Package Details",
        Foreground = textBrush,
        Margin = new Thickness(0, 10, 0, 10),
        FontSize = 17
      };

      #endregion

      #region Instance name panel

      var instanceNamePanel = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };
      var instanceNameLabel = new Label
      {
        Content = "Instance name: ",
        Width = 120,
        Foreground = textBrush,
        FontSize = 13
      };
      var instanceNameValue = new Label
      {
        FontSize = 13
      };
      try
      {
        instanceNameValue.Content = Context.SitecoreInfo.InstanceName;
      }
      catch
      {
        instanceNameValue.Content = Strings.DataIsNotAvailable;
      }
      instanceNamePanel.Children.Add(instanceNameLabel);
      instanceNamePanel.Children.Add(instanceNameValue);

      #endregion

      #region Sitecore version panel

      var sitecoreVersionPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };
      var sitecoreVersionLabel = new Label
      {
        Content = "Sitecore version: ",
        Width = 120,
        Foreground = textBrush,
        FontSize = 13
      };
      var sitecoreVersionValue = new Label
      {
        FontSize = 13
      };
      try
      {
        sitecoreVersionValue.Content = Context.SitecoreInfo.SitecoreVersion.ToString();
      }
      catch
      {
        sitecoreVersionValue.Content = Strings.DataIsNotAvailable;
      }
      sitecoreVersionPanel.Children.Add(sitecoreVersionLabel);
      sitecoreVersionPanel.Children.Add(sitecoreVersionValue);

      #endregion

      #region SQL databases panel

      var sqlDatabasesPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };
      var sqlDatabasesLabel = new Label
      {
        Content = "SQL databases: ",
        Width = 120,
        Foreground = textBrush,
        FontSize = 13
      };
      var sqlDatabasesValue = new Label
      {
        FontSize = 13
      };
      try
      {
        sqlDatabasesValue.Content = string.Join(", ", Context.Databases.Sql.DatabaseNames);
      }
      catch
      {
        sqlDatabasesValue.Content = Strings.DataIsNotAvailable;
      }
      sqlDatabasesPanel.Children.Add(sqlDatabasesLabel);
      sqlDatabasesPanel.Children.Add(sqlDatabasesValue);

      #endregion

      #region Mongo databases panel

      var mongoDatabasesPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };
      var mongoDatabasesLabel = new Label
      {
        Content = "Mongo databases: ",
        Width = 120,
        Foreground = textBrush,
        FontSize = 13
      };
      var mongoDatabasesValue = new Label
      {
        FontSize = 13
      };
      try
      {
        mongoDatabasesValue.Content = string.Join(", ", Context.Databases.Mongo.DatabaseNames);
      }
      catch
      {
        mongoDatabasesValue.Content = Strings.DataIsNotAvailable;
      }
      mongoDatabasesPanel.Children.Add(mongoDatabasesLabel);
      mongoDatabasesPanel.Children.Add(mongoDatabasesValue);

      #endregion

      #region Modules panel

      var modulesPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };
      var modulesLabel = new Label
      {
        Content = "Modules: ",
        Width = 120,
        Foreground = textBrush,
        FontSize = 13
      };
      var modulesValue = new Label
      {
        FontSize = 13
      };
      try
      {
        modulesValue.Content = string.Join(" ", Context.SitecoreInfo.ModulesInformation.InstalledModules.Keys);
      }
      catch
      {
        modulesValue.Content = Strings.DataIsNotAvailable;
      }
      modulesPanel.Children.Add(modulesLabel);
      modulesPanel.Children.Add(modulesValue);

      #endregion

      #region Add components

      mainStackPanel.Children.Add(header);
      mainStackPanel.Children.Add(instanceNamePanel);
      mainStackPanel.Children.Add(sitecoreVersionPanel);
      mainStackPanel.Children.Add(sqlDatabasesPanel);
      mainStackPanel.Children.Add(mongoDatabasesPanel);
      mainStackPanel.Children.Add(modulesPanel);
      border.Child = mainStackPanel;
      popup.Child = border;

      #endregion

      popup.PlacementTarget = View.InfoIcon;
      popup.IsOpen = true;
    }

    public ICommand LoadPackageCommand
    {
      get
      {
        return new RelayCommand(() =>
          {
            var openFileDialog = new OpenFileDialog
            {
              Filter = "SSPG package|*.zip"
            };
            if (openFileDialog.ShowDialog() != true)
            {
              return;
            }

            InitilizePackgeDetails(openFileDialog.FileName);
            PackagePath = openFileDialog.FileName;
          },
          () => true);
      }
    }

    public string SitecoreVersion { get; set; }

    public string MachineName { get; set; }

    public string InstanceName { get; set; }

    public ICommand ServerRolesChangedCommand
    {
      get
      {
        return new RelayCommand(() => { ServerRoles = View.ServerRolesCheckList.SelectedItems.Cast<string>().Select(r => (ServerRole)Enum.Parse(typeof(ServerRole), r)).ToList(); },
          () => true);
      }
    }

    #endregion

    #region Properties

    public ResourceDetailsPageView View { get; set; }
    public bool IsSitecoreVersionValid => !string.IsNullOrEmpty(SitecoreVersion) && !SitecoreVersion.Equals(Strings.DataIsNotAvailable, StringComparison.OrdinalIgnoreCase);
    public bool IsPackageLoaded => !string.IsNullOrEmpty(PackagePath);
    public bool IsResourceValid => IsPackageLoaded && ServerRoles != null && ServerRoles.Any() && IsSitecoreVersionValid;

    public string PackagePath
    {
      get
      {
        return packagePath;
      }
      set
      {
        packagePath = value;
        OnPropertyChanged("PackagePath");
        OnPropertyChanged("IsPackageLoaded");
        OnPropertyChanged("IsResourceValid");
        OnPropertyChanged("MachineName");
        OnPropertyChanged("InstanceName");
        OnPropertyChanged("SitecoreVersion");
      }
    }

    public List<ServerRole> ServerRoles
    {
      get
      {
        return serverRoles;
      }
      set
      {
        serverRoles = value;
        OnPropertyChanged("IsResourceValid");
      }
    }

    #endregion

    #region Private methods

    private void InitilizePackgeDetails(string fileName)
    {
      try
      {
        var testRunner = new TestRunner();
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        Context = testRunner.CreateContext(new SupportPackageDataProvider(fileName, ServerRoles, null, null, $"{assemblyName.Name}, {assemblyName.Version.ToString()}"));

        try
        {
          InstanceName = Context.SitecoreInfo.InstanceName;
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to extract instance name");

          InstanceName = Strings.DataIsNotAvailable;
        }

        try
        {
          MachineName = Context.WebServer.Info.MachineName;
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to extract machine name");

          MachineName = Strings.DataIsNotAvailable;
        }

        try
        {
          SitecoreVersion = Context.SitecoreInfo.SitecoreVersion.ToString();
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to extract sitecore version");

          SitecoreVersion = Strings.DataIsNotAvailable;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Failed create context");

        InstanceName = MachineName = SitecoreVersion = Strings.DataIsNotAvailable;
      }
    }

    #endregion Private methods

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}