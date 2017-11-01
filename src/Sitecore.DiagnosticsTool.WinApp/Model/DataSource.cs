namespace Sitecore.DiagnosticsTool.WinApp.Model
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Runtime.CompilerServices;

  using Sitecore.DiagnosticsTool.Core.Categories;

  public class DataSource : INotifyPropertyChanged
  {
    #region Properties

    public List<Category> Categories { get; set; }

    public string ReportPath { get; set; }

    public string ErrorMessage { get; set; }

    public List<SourcePackageModel> Packages { get; set; } = new List<SourcePackageModel>();

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