namespace Sitecore.DiagnosticsTool.WinApp.Model
{
  /// <summary>
  ///   Sync global DataSoure with a data collected by a paricular PageViewModel
  /// </summary>
  interface ISyncableModel
  {
    void Sync();
  }
}