namespace Sitecore.DiagnosticsTool.WinApp.Model
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Runtime.CompilerServices;

  using Sitecore.DiagnosticsTool.Core.Categories;

  public class SourcePackageModel : INotifyPropertyChanged
  {
    #region Properties

    public List<ServerRole> Roles { get; set; }

    public string Path { get; set; }

    #endregion

    #region INotifyPropertyChanged members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}