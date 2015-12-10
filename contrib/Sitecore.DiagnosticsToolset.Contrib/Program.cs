namespace Sitecore.DiagnosticsToolset.Contrib
{
  using System;
  using System.Diagnostics;

  public static class Program
  {
    public static void Main()
    {
      // Sitecore.DiagnosticsToolset.WinApp.exe will check all assemblies in the same folder 
      // and pick the test classes that implement ITest (or derive from abstract Test class)

      Console.WriteLine("Running Sitecore.DiagnosticsToolset.WinApp.exe...");
      Process.Start("Sitecore.DiagnosticsToolset.WinApp.exe").WaitForExit();
    }
  }
}
