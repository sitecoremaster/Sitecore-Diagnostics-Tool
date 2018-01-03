namespace Sitecore.DiagnosticsTool.Commands
{
  using Sitecore.Diagnostics.FileSystem;

  public interface INewCommand
  {
    string[] Options { get; }

    IFileSystem FileSystem { get; }

    void Execute();
  }
}