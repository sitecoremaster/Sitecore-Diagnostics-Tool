namespace Sitecore.DiagnosticsTool.Commands
{
  using Sitecore.Diagnostics.FileSystem;

  internal interface IRunCommand
  {
    string[] Options { get; }

    IFileSystem FileSystem { get; }

    void Execute();
  }
}